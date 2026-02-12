using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Order;
using BuildingBlocks.ApiClients.Clients.Order.Booking.Models;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Calendars.Models;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Calendars.Queries.GetCalendar;

public record GetWorkingTimeTechnicianByStoreIdQuery : IRequest<ApiResponse<WorkingTimeDto>>
{
    public int StoreId { get; init; }
    public int TechnicianId { get; init; }
    public DateTime Date { get; init; }
}

public class GetWorkingTimeTechnicianByStoreIdQueryHandler : IRequestHandler<GetWorkingTimeTechnicianByStoreIdQuery, ApiResponse<WorkingTimeDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IOrderClient _orderClient;

    public GetWorkingTimeTechnicianByStoreIdQueryHandler(ICatalogDbContext context, IMapper mapper, IOrderClient orderClient)
    {
        _context = context;
        _mapper = mapper;
        _orderClient = orderClient;
    }

    public async Task<ApiResponse<WorkingTimeDto>> Handle(GetWorkingTimeTechnicianByStoreIdQuery request, CancellationToken cancellationToken)
    {
        var targetDate = request.Date.Date;

        var schedules = await _context.Calendar
            .Include(x => x.CalendarType)
            .Include(x => x.CalendarOverrides)
            .ThenInclude(x => x.CalendarType)
            .Where(ws =>
                ws.TechnicianId == request.TechnicianId &&
                ws.StoreId == request.StoreId &&
                !ws.IsDeleted)
            .ToListAsync(cancellationToken);

        var overrideEntries = schedules
            .SelectMany(c => c.CalendarOverrides)
            .Where(o => o.WorkDate.Date == targetDate && !o.IsDeleted)
            .ToList();
        
        var offOverrides = overrideEntries
            .Where(o => IsOff(o.CalendarType.Name))
            .ToList();

        var workingOverrides = overrideEntries
            .Where(o => !IsOff(o.CalendarType.Name))
            .ToList();
        // Chỉ OFF toàn ngày khi không tồn tại Working override
        if (offOverrides.Any() && !workingOverrides.Any())
        {
            return ApiResponse<WorkingTimeDto>.Error("Technician is off on this day.");
        }
        var workingTimes = new List<WorkingTimeDto>();

        if (workingOverrides.Any())
        {
            workingTimes = workingOverrides.Select(o => new WorkingTimeDto
            {
                StoreId = request.StoreId,
                TechnicianId = request.TechnicianId,
                WorkDate = targetDate,
                WorkStartTime = o.WorkStartTime,
                WorkEndTime = o.WorkEndTime
            }).ToList();
        }
        else
        {
            var overrideDates = schedules
                .SelectMany(c => c.CalendarOverrides)
                .Where(o => o.WorkDate.Date == targetDate)
                .Select(o => o.WorkDate.Date)
                .ToHashSet();

            var matchedSchedules = schedules
                .Where(ws =>
                {
                    if (overrideDates.Contains(targetDate))
                        return false;

                    if (ws.Recurrence == null || ws.Recurrence == RecurrenceType.None)
                        return ws.WorkDate.Date == targetDate;

                    if (ws.WorkDate.Date > targetDate)
                        return false;

                    return ws.Recurrence switch
                    {
                        RecurrenceType.Daily => true,
                        RecurrenceType.Weekly => ws.WorkDate.DayOfWeek == targetDate.DayOfWeek,
                        RecurrenceType.Monthly => ws.WorkDate.Day == targetDate.Day,
                        RecurrenceType.Yearly => ws.WorkDate.Day == targetDate.Day && ws.WorkDate.Month == targetDate.Month,
                        _ => false
                    };
                })
                .ToList();

            workingTimes = matchedSchedules.Select(ws => new WorkingTimeDto
            {
                StoreId = request.StoreId,
                TechnicianId = request.TechnicianId,
                WorkDate = targetDate,
                WorkStartTime = ws.WorkStartTime,
                WorkEndTime = ws.WorkEndTime
            }).ToList();
        }

        if (!workingTimes.Any())
        {
            return ApiResponse<WorkingTimeDto>.Error("There are no schedules for this day.");
        }

        var earliestStart = workingTimes.Min(x => x.WorkStartTime);
        var latestEnd = workingTimes.Max(x => x.WorkEndTime);
        // Step: Lấy danh sách đã booking
        var bookedResponse = await _orderClient.GetBookedSlotsAsync(
            request.StoreId,
            request.TechnicianId,
            targetDate,
            cancellationToken
        );

        var bookedSlots = bookedResponse?.Data != null
            ? bookedResponse.Data.ToList()
            : new List<BookingTimeDto>();
        var availableSlots = new List<string>();

        foreach (var time in workingTimes)
        {
            var slots = SplitIntoSlots(time, slotMinutes: 60);
            foreach (var slot in slots)
            {
                var isBooked = bookedSlots.Any(b =>
                    b.BookingTime == slot.WorkStartTime);

                if (!isBooked)
                {
                    availableSlots.Add(slot.WorkStartTime.ToString(@"hh\:mm"));
                }
            }
        }

        var result = new WorkingTimeDto
        {
            WorkStartTime = earliestStart,
            WorkEndTime = latestEnd,
            AvailableSlots = availableSlots,
            WorkDate = targetDate,
            StoreId = request.StoreId,
            TechnicianId = request.TechnicianId,
        };

        return ApiResponse<WorkingTimeDto>.Success(_mapper.Map<WorkingTimeDto>(result));
    }

    private List<WorkingTimeDto> SplitIntoSlots(WorkingTimeDto time, int slotMinutes)
    {
        var result = new List<WorkingTimeDto>();
        var start = time.WorkStartTime;
        var end = time.WorkEndTime;

        while (start < end)
        {
            var next = start.Add(TimeSpan.FromMinutes(slotMinutes));
            if (next > end) break;

            result.Add(new WorkingTimeDto
            {
                StoreId = time.StoreId,
                TechnicianId = time.TechnicianId,
                WorkDate = time.WorkDate,
                WorkStartTime = start,
                WorkEndTime = next
            });

            start = next;
        }

        return result;
    }
    private static bool IsOff(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;

        name = name.Trim().ToLowerInvariant();

        return name.Contains("off")
            || name.Contains("absent");
    }
}
