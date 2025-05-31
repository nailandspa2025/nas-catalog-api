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

    private List<CalendarDto> ExpandRecurrence(Calendar calendar, DateTime startDate, DateTime endDate)
    {
        var occurrences = new List<CalendarDto>();
        //var overrides = calendar.CalendarOverrides?
        //  .Where(o => !o.IsDeleted)
        //  .ToDictionary(o => o.WorkDate.Date) ?? new Dictionary<DateTime, CalendarOverride>();
        var overrides = calendar.CalendarOverrides?
            .Where(o => !o.IsDeleted)
            .GroupBy(o => o.WorkDate.Date)
            .ToDictionary(g => g.Key, g => g.ToList())
            ?? new Dictionary<DateTime, List<CalendarOverride>>();

        if (calendar.Recurrence == null || calendar.Recurrence == RecurrenceType.None)
        {
            if (calendar.WorkDate >= startDate && calendar.WorkDate <= endDate)
            {
                occurrences.Add(ToDto(calendar, workDateOverride: calendar.WorkDate));
            }
            return occurrences;
        }
        var current = calendar.WorkDate;
        var interval = calendar.RecurrenceInterval ?? 1;
        var recurrenceEnd = calendar.RecurrenceEndDate ?? endDate;

        while (current <= endDate)
        {
            if (current >= startDate)
            {
                //if (calendar.CalendarOverrides?.Any(o => o.WorkDate.Date == current.Date && o.IsDeleted) == true)
                //{
                //    // Skip this occurrence (marked as deleted)
                //}
                //else if (overrides.TryGetValue(current.Date, out var overrideEntry))
                //{
                //    occurrences.Add(ToDto(calendar, overrideEntry));
                //}
                //else
                //{
                //    occurrences.Add(ToDto(calendar, workDateOverride: current));
                //}
                var isDeleted = calendar.CalendarOverrides?.Any(o => o.WorkDate.Date == current.Date && o.IsDeleted) == true;
                if (!isDeleted)
                {
                    if (overrides.TryGetValue(current.Date, out var overrideEntries))
                    {
                        // Nếu có nhiều override cùng ngày thì add tất cả
                        foreach (var overrideEntry in overrideEntries)
                        {
                            occurrences.Add(ToDto(calendar, overrideEntry));
                        }
                    }
                    else
                    {
                        occurrences.Add(ToDto(calendar, workDateOverride: current));
                    }
                }
            }

            current = calendar.Recurrence switch
            {
                RecurrenceType.Daily => current.AddDays(interval),
                RecurrenceType.Weekly => current.AddDays(7 * interval),
                RecurrenceType.Monthly => current.AddMonths(interval),
                RecurrenceType.Yearly => current.AddYears(interval),
                _ => current
            };
        }

        return occurrences;
    }

    private CalendarDto ToDto(Calendar calendar, CalendarOverride? overrideEntry = null, DateTime? workDateOverride = null)
    {
        var dto = _mapper.Map<CalendarDto>(calendar);
        dto.Id = Guid.NewGuid();
        dto.WorkDate = overrideEntry?.WorkDate ?? workDateOverride ?? calendar.WorkDate;
        if(overrideEntry != null)
        {
            dto.Color = overrideEntry.CalendarType?.Color;
            dto.Title = overrideEntry?.Title ?? calendar.Title;
            dto.Description = overrideEntry.Description;
            dto.WorkStartTime = overrideEntry.WorkStartTime;
            dto.WorkEndTime = overrideEntry.WorkEndTime;
            dto.TechnicianId = overrideEntry.TechnicianId;
            dto.StoreId = overrideEntry.StoreId;
        }
        return dto;
    }
}