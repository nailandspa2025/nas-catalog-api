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

    private List<CalendarDto> ExpandRecurrence(Calendar calendar, DateTime startDate, DateTime endDate)
    {
        var occurrences = new List<CalendarDto>();

        var overrides = calendar.CalendarOverrides?
            .Where(o => !o.IsDeleted)
            .GroupBy(o => o.WorkDate.Date)
            .ToDictionary(
                g => g.Key,
                g => g.ToList())
            ?? new Dictionary<
                DateTime,
                List<CalendarOverride>>();

        // --------------------------------------------------------
        // No recurrence
        // --------------------------------------------------------

        if (calendar.Recurrence == null ||
            calendar.Recurrence == RecurrenceType.None)
        {
            if (calendar.WorkDate.Date >= startDate.Date &&
                calendar.WorkDate.Date <= endDate.Date)
            {
                if (overrides.TryGetValue(
                        calendar.WorkDate.Date,
                        out var overrideEntries))
                {
                    foreach (var overrideEntry in overrideEntries)
                    {
                        occurrences.Add(
                            ToDto(calendar, overrideEntry));
                    }
                }
                else
                {
                    occurrences.Add(
                        ToDto(
                            calendar,
                            workDateOverride: calendar.WorkDate));
                }
            }

            return occurrences;
        }


        // --------------------------------------------------------
        // Recurrence interval
        //
        // Dữ liệu cũ không có interval => 1
        // --------------------------------------------------------

        var interval = calendar.RecurrenceInterval ?? 1;

        if (interval <= 0)
        {
            interval = 1;
        }


        // --------------------------------------------------------
        // Recurrence end
        // --------------------------------------------------------

        var recurrenceEnd =
            calendar.RecurrenceEndDate?.Date
            ?? endDate.Date;

        if (recurrenceEnd > endDate.Date)
        {
            recurrenceEnd = endDate.Date;
        }

        if (recurrenceEnd < startDate.Date)
        {
            return occurrences;
        }


        // ========================================================
        // Daily
        // ========================================================

        if (calendar.Recurrence == RecurrenceType.Daily)
        {
            var current = calendar.WorkDate.Date;

            while (current <= recurrenceEnd)
            {
                if (current >= startDate.Date)
                {
                    AddOccurrence(
                        occurrences,
                        calendar,
                        overrides,
                        current);
                }

                current = current.AddDays(interval);
            }

            return occurrences;
        }


        // ========================================================
        // Weekly
        //
        // Weekly sử dụng DayOfWeek của WorkDate.
        //
        // Ví dụ:
        // WorkDate = Wednesday
        // Interval = 2
        //
        // => mỗi 2 tuần vào Wednesday
        // ========================================================

        if (calendar.Recurrence == RecurrenceType.Weekly)
        {
            var current = calendar.WorkDate.Date;

            while (current <= recurrenceEnd)
            {
                if (current >= startDate.Date &&
                    current.DayOfWeek ==
                    calendar.WorkDate.DayOfWeek)
                {
                    AddOccurrence(
                        occurrences,
                        calendar,
                        overrides,
                        current);
                }

                current = current.AddDays(7 * interval);
            }

            return occurrences;
        }
        // ========================================================
        // DayOfWeek / WorkingDays
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
        // Interval = số tuần
        // ========================================================

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

            var currentWeek =
                StartOfWeek(
                    calendar.WorkDate.Date,
                    DayOfWeek.Monday);

            while (currentWeek <= recurrenceEnd)
            {
                var weekDifference =
                    (currentWeek -
                     StartOfWeek(
                         calendar.WorkDate.Date,
                         DayOfWeek.Monday))
                    .Days / 7;

                if (weekDifference % interval == 0)
                {
                    for (var dayOffset = 0;
                         dayOffset < 7;
                         dayOffset++)
                    {
                        var current =
                            currentWeek.AddDays(dayOffset);

                        if (current < calendar.WorkDate.Date)
                        {
                            continue;
                        }

                        if (current > recurrenceEnd)
                        {
                            continue;
                        }

                        if (current < startDate.Date ||
                            current > endDate.Date)
                        {
                            continue;
                        }

                        var dayOfWeek =
                            ToDayOfWeekValue(
                                current.DayOfWeek);

                        if (!selectedDays.Contains(dayOfWeek))
                        {
                            continue;
                        }

                        AddOccurrence(
                            occurrences,
                            calendar,
                            overrides,
                            current);
                    }
                }

                currentWeek =
                    currentWeek.AddDays(7);
            }

            return occurrences;
        }


        // ========================================================
        // Monthly
        // ========================================================

        if (calendar.Recurrence == RecurrenceType.Monthly)
        {
            var current = calendar.WorkDate.Date;

            while (current <= recurrenceEnd)
            {
                if (current >= startDate.Date &&
                    current.Day == calendar.WorkDate.Day)
                {
                    AddOccurrence(
                        occurrences,
                        calendar,
                        overrides,
                        current);
                }

                current = current.AddMonths(interval);
            }

            return occurrences;
        }


        // ========================================================
        // Yearly
        // ========================================================

        if (calendar.Recurrence == RecurrenceType.Yearly)
        {
            var current = calendar.WorkDate.Date;

            while (current <= recurrenceEnd)
            {
                if (current >= startDate.Date &&
                    current.Day == calendar.WorkDate.Day &&
                    current.Month == calendar.WorkDate.Month)
                {
                    AddOccurrence(
                        occurrences,
                        calendar,
                        overrides,
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

    private static int ToDayOfWeekValue(
        DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => 0,
            DayOfWeek.Monday => 1,
            DayOfWeek.Tuesday => 2,
            DayOfWeek.Wednesday => 3,
            DayOfWeek.Thursday => 4,
            DayOfWeek.Friday => 5,
            DayOfWeek.Saturday => 6,

            _ => 0
        };
    }


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