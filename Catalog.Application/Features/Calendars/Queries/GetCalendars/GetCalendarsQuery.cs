using AutoMapper;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Calendars.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Calendars.Queries.GetCalendars;

public record GetCalendarsQuery: IRequest<ApiResponse<List<CalendarDto>>>
{
    public DateTime StartDate { get; init; }

    public DateTime EndDate { get; init; }

    public List<long>? StoreIds { get; init; }

    public long? TechnicianId { get; init; }

}

public class GetCalendarsQueryHandler : IRequestHandler<GetCalendarsQuery, ApiResponse<List<CalendarDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public GetCalendarsQueryHandler(ICatalogDbContext context, IMapper mapper, ICurrentUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<List<CalendarDto>>> Handle(GetCalendarsQuery request, CancellationToken cancellationToken)
    {
        var startDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
        var endDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);
        var query = _context.Calendar
            .Include(c => c.CalendarType)
            .Include(c => c.Store)
            .Include(c => c.CalendarOverrides)
                .ThenInclude(o => o.CalendarType)
            .Include(x => x.DaysOfWeek)
            .Where(c => c.WorkDate <= endDate && !c.IsDeleted && c.CreatedBy == _currentUser.UserName);

        if (request.StoreIds != null && request.StoreIds.Any())
        {
            query = query.Where(c => request.StoreIds.Contains(c.StoreId));
        }

        if (request.TechnicianId.HasValue)
        {
            query = query.Where(c => c.TechnicianId == request.TechnicianId.Value);
        }
        var calendarList = await query.ToListAsync(cancellationToken);
        var calendars = calendarList
            .SelectMany(c => ExpandRecurrence(c, startDate, endDate))
            .Where(c => c.WorkDate >= startDate && c.WorkDate <= endDate)
            .OrderBy(c => c.WorkDate)
            .ToList();

        return ApiResponse<List<CalendarDto>>.Success(calendars);
    }

    // private List<CalendarDto> ExpandRecurrence(Calendar calendar, DateTime startDate, DateTime endDate)
    // {
    //     var occurrences = new List<CalendarDto>();
    //     //var overrides = calendar.CalendarOverrides?
    //     //  .Where(o => !o.IsDeleted)
    //     //  .ToDictionary(o => o.WorkDate.Date) ?? new Dictionary<DateTime, CalendarOverride>();
    //     var overrides = calendar.CalendarOverrides?
    //         .Where(o => !o.IsDeleted)
    //         .GroupBy(o => o.WorkDate.Date)
    //         .ToDictionary(g => g.Key, g => g.ToList())
    //         ?? new Dictionary<DateTime, List<CalendarOverride>>();

    //     if (calendar.Recurrence == null || calendar.Recurrence == RecurrenceType.None)
    //     {
    //         if (calendar.WorkDate >= startDate && calendar.WorkDate <= endDate)
    //         {
    //             occurrences.Add(ToDto(calendar, workDateOverride: calendar.WorkDate));
    //         }
    //         return occurrences;
    //     }
    //     var current = calendar.WorkDate;
    //     var interval = calendar.RecurrenceInterval ?? 1;
    //     var recurrenceEnd = calendar.RecurrenceEndDate ?? endDate;

    //     while (current <= endDate)
    //     {
    //         if (current >= startDate)
    //         {
    //             //if (calendar.CalendarOverrides?.Any(o => o.WorkDate.Date == current.Date && o.IsDeleted) == true)
    //             //{
    //             //    // Skip this occurrence (marked as deleted)
    //             //}
    //             //else if (overrides.TryGetValue(current.Date, out var overrideEntry))
    //             //{
    //             //    occurrences.Add(ToDto(calendar, overrideEntry));
    //             //}
    //             //else
    //             //{
    //             //    occurrences.Add(ToDto(calendar, workDateOverride: current));
    //             //}
    //             var isDeleted = calendar.CalendarOverrides?.Any(o => o.WorkDate.Date == current.Date && o.IsDeleted) == true;
    //             if (!isDeleted)
    //             {
    //                 if (overrides.TryGetValue(current.Date, out var overrideEntries))
    //                 {
    //                     // Nếu có nhiều override cùng ngày thì add tất cả
    //                     foreach (var overrideEntry in overrideEntries)
    //                     {
    //                         occurrences.Add(ToDto(calendar, overrideEntry));
    //                     }
    //                 }
    //                 else
    //                 {
    //                     occurrences.Add(ToDto(calendar, workDateOverride: current));
    //                 }
    //             }
    //         }

    //         current = calendar.Recurrence switch
    //         {
    //             RecurrenceType.Daily => current.AddDays(interval),
    //             RecurrenceType.Weekly => current.AddDays(7 * interval),
    //             RecurrenceType.Monthly => current.AddMonths(interval),
    //             RecurrenceType.Yearly => current.AddYears(interval),
    //             _ => current
    //         };
    //     }

    //     return occurrences;
    // }
    // ============================================================
    // Expand Calendar recurrence
    // ============================================================

    private List<CalendarDto> ExpandRecurrence(
    Calendar calendar,
    DateTime startDate,
    DateTime endDate)
{
    var occurrences = new List<CalendarDto>();

    // ============================================================
    // Normalize date
    // ============================================================

    var calendarStartDate = calendar.WorkDate.Date;
    var queryStartDate = startDate.Date;
    var queryEndDate = endDate.Date;

    // ============================================================
    // Calendar Overrides
    // ============================================================

    var deletedDates = calendar.CalendarOverrides?
        .Where(x => x.IsDeleted)
        .Select(x => x.WorkDate.Date)
        .ToHashSet()
        ?? new HashSet<DateTime>();

    var overrides = calendar.CalendarOverrides?
        .Where(x => !x.IsDeleted)
        .GroupBy(x => x.WorkDate.Date)
        .ToDictionary(
            g => g.Key,
            g => g.ToList())
        ?? new Dictionary<DateTime, List<CalendarOverride>>();

    // ============================================================
    // No recurrence
    // ============================================================

    if (calendar.Recurrence is null ||
        calendar.Recurrence == RecurrenceType.None)
    {
        var date = calendarStartDate;

        if (date < queryStartDate || date > queryEndDate)
        {
            return occurrences;
        }

        // Ngày đã bị delete
        if (deletedDates.Contains(date))
        {
            return occurrences;
        }

        AddOccurrence(
            occurrences,
            calendar,
            overrides,
            date);

        return occurrences;
    }

    // ============================================================
    // Recurrence interval
    // ============================================================

    var interval = calendar.RecurrenceInterval ?? 1;

    if (interval <= 0)
    {
        interval = 1;
    }

    // ============================================================
    // Recurrence end
    // ============================================================

    var recurrenceEnd = calendar.RecurrenceEndDate?.Date
        ?? queryEndDate;

    if (recurrenceEnd > queryEndDate)
    {
        recurrenceEnd = queryEndDate;
    }

    if (recurrenceEnd < queryStartDate)
    {
        return occurrences;
    }

    // ============================================================
    // Daily
    // ============================================================

    if (calendar.Recurrence == RecurrenceType.Daily)
    {
        var current = calendarStartDate;

        while (current <= recurrenceEnd)
        {
            if (current >= queryStartDate)
            {
                AddOccurrence(
                    occurrences,
                    calendar,
                    overrides,
                    deletedDates,
                    current);
            }

            current = current.AddDays(interval);
        }

        return occurrences;
    }

    // ============================================================
    // Weekly
    //
    // Weekly = lặp đúng thứ của WorkDate
    //
    // Ví dụ:
    // WorkDate = Tuesday
    // Interval = 2
    //
    // => mỗi 2 tuần vào Tuesday
    // ============================================================

    if (calendar.Recurrence == RecurrenceType.Weekly)
    {
        var current = calendarStartDate;

        while (current <= recurrenceEnd)
        {
            if (current >= queryStartDate &&
                current.DayOfWeek == calendarStartDate.DayOfWeek)
            {
                AddOccurrence(
                    occurrences,
                    calendar,
                    overrides,
                    deletedDates,
                    current);
            }

            current = current.AddDays(7 * interval);
        }

        return occurrences;
    }

    // ============================================================
    // Working Days / DayOfWeek
    //
    // DaysOfWeek:
    //
    // Monday    = 1
    // Tuesday   = 2
    // Wednesday = 3
    // Thursday  = 4
    // Friday    = 5
    // Saturday  = 6
    // Sunday    = 0
    //
    // interval = số tuần
    // ============================================================

    if (calendar.Recurrence == RecurrenceType.DayOfWeek)
    {
        var selectedDays = calendar.DaysOfWeek?
            .Select(x => (int)x.DayOfWeek)
            .Distinct()
            .ToHashSet()
            ?? new HashSet<int>();

        if (!selectedDays.Any())
        {
            return occurrences;
        }

        var baseWeek = StartOfWeek(
            calendarStartDate,
            DayOfWeek.Monday);

        var currentWeek = baseWeek;

        while (currentWeek <= recurrenceEnd)
        {
            var weekDifference =
                (currentWeek - baseWeek).Days / 7;

            if (weekDifference % interval == 0)
            {
                for (var dayOffset = 0; dayOffset < 7; dayOffset++)
                {
                    var current = currentWeek.AddDays(dayOffset);

                    if (current < calendarStartDate)
                    {
                        continue;
                    }

                    if (current > recurrenceEnd)
                    {
                        continue;
                    }

                    if (current < queryStartDate ||
                        current > queryEndDate)
                    {
                        continue;
                    }

                    var dayOfWeek = (int)current.DayOfWeek;

                    if (!selectedDays.Contains(dayOfWeek))
                    {
                        continue;
                    }

                    AddOccurrence(
                        occurrences,
                        calendar,
                        overrides,
                        deletedDates,
                        current);
                }
            }

            currentWeek = currentWeek.AddDays(7);
        }

        return occurrences;
    }

    // ============================================================
    // Monthly
    // ============================================================

    if (calendar.Recurrence == RecurrenceType.Monthly)
    {
        var current = calendarStartDate;

        while (current <= recurrenceEnd)
        {
            if (current >= queryStartDate &&
                current.Day == calendarStartDate.Day)
            {
                AddOccurrence(
                    occurrences,
                    calendar,
                    overrides,
                    deletedDates,
                    current);
            }

            current = current.AddMonths(interval);
        }

        return occurrences;
    }

    // ============================================================
    // Yearly
    // ============================================================

    if (calendar.Recurrence == RecurrenceType.Yearly)
    {
        var current = calendarStartDate;

        while (current <= recurrenceEnd)
        {
            if (current >= queryStartDate &&
                current.Day == calendarStartDate.Day &&
                current.Month == calendarStartDate.Month)
            {
                AddOccurrence(
                    occurrences,
                    calendar,
                    overrides,
                    deletedDates,
                    current);
            }

            current = current.AddYears(interval);
        }

        return occurrences;
    }

    return occurrences;
}
    // ============================================================
    // Add occurrence
    // ============================================================
    private void AddOccurrence(
    List<CalendarDto> occurrences,
    Calendar calendar,
    Dictionary<DateTime, List<CalendarOverride>> overrides,
    HashSet<DateTime> deletedDates,
    DateTime date)
{
    var workDate = date.Date;

    // ============================================================
    // Ngày này đã bị delete
    // ============================================================

    if (deletedDates.Contains(workDate))
    {
        return;
    }

    // ============================================================
    // Có override
    // ============================================================

    if (overrides.TryGetValue(
            workDate,
            out var overrideEntries))
    {
        foreach (var overrideEntry in overrideEntries)
        {
            occurrences.Add(
                ToDto(
                    calendar,
                    overrideEntry));
        }

        return;
    }

    // ============================================================
    // Calendar gốc
    // ============================================================

    occurrences.Add(
        ToDto(
            calendar,
            workDateOverride: workDate));
}
    private void AddOccurrence(List<CalendarDto> occurrences, Calendar calendar,Dictionary<DateTime, List<CalendarOverride>> overrides, DateTime date)
    {
        // --------------------------------------------------------
        // Có override cho ngày này
        // --------------------------------------------------------

        if (overrides.TryGetValue(
                date.Date,
                out var overrideEntries))
        {
            foreach (var overrideEntry in overrideEntries)
            {
                occurrences.Add(
                    ToDto(
                        calendar,
                        overrideEntry));
            }

            return;
        }


        // --------------------------------------------------------
        // Không có override
        // --------------------------------------------------------

        occurrences.Add(
            ToDto(
                calendar,
                workDateOverride: date));
    }

    private CalendarDto ToDto(Calendar calendar, CalendarOverride? overrideEntry = null, DateTime? workDateOverride = null)
    {
        var dto = _mapper.Map<CalendarDto>(calendar);
        dto.Id = Guid.NewGuid();
        dto.WorkDate = overrideEntry?.WorkDate ?? workDateOverride ?? calendar.WorkDate;
        //dto.WorkDate =overrideEntry?.WorkDate?? workDateOverride?? calendar.WorkDate;
        if (overrideEntry != null)
        {
            dto.Color =
                overrideEntry.CalendarType?.Color
                ?? calendar.CalendarType?.Color;

            dto.Title =
                overrideEntry.Title
                ?? calendar.Title;

            dto.Description =
                overrideEntry.Description
                ?? calendar.Description;

            dto.WorkStartTime =
                overrideEntry.WorkStartTime;

            dto.WorkEndTime =
                overrideEntry.WorkEndTime;

            dto.TechnicianId =
                overrideEntry.TechnicianId;

            dto.StoreId =
                overrideEntry.StoreId;

            dto.CalendarTypeId =
                overrideEntry.CalendarTypeId;
        }
        return dto;
    }
    // ============================================================
    // DayOfWeek mapping
    // ============================================================

    // private static int ToDayOfWeekValue(
    //     DayOfWeek dayOfWeek)
    // {
    //     return dayOfWeek switch
    //     {
    //         DayOfWeek.Sunday => 0,
    //         DayOfWeek.Monday => 1,
    //         DayOfWeek.Tuesday => 2,
    //         DayOfWeek.Wednesday => 3,
    //         DayOfWeek.Thursday => 4,
    //         DayOfWeek.Friday => 5,
    //         DayOfWeek.Saturday => 6,

    //         _ => 0
    //     };
    // }


    // ============================================================
    // Start of week
    // ============================================================

    private static DateTime StartOfWeek(
        DateTime date,
        DayOfWeek startOfWeek)
    {
        var diff =
            (7 + (date.DayOfWeek - startOfWeek)) % 7;

        return date.Date.AddDays(-diff);
    }
}