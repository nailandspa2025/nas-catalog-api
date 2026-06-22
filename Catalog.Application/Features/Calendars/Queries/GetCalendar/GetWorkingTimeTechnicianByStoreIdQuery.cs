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
    public List<int> ServiceIds { get; set; } = new();
}

public class GetWorkingTimeTechnicianByStoreIdQueryHandler : IRequestHandler<GetWorkingTimeTechnicianByStoreIdQuery, ApiResponse<WorkingTimeDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IOrderClient _orderClient;

    private static readonly TimeSpan Gap = TimeSpan.FromMinutes(5); // Khoảng cách tối thiểu

    public GetWorkingTimeTechnicianByStoreIdQueryHandler(
        ICatalogDbContext context,
        IMapper mapper,
        IOrderClient orderClient)
    {
        _context = context;
        _mapper = mapper;
        _orderClient = orderClient;
    }

    public async Task<ApiResponse<WorkingTimeDto>> Handle(
        GetWorkingTimeTechnicianByStoreIdQuery request,
        CancellationToken cancellationToken)
    {
        var targetDate = request.Date.Date;

        // 1. Kiểm tra ServiceIds từ request
        if (request.ServiceIds == null || !request.ServiceIds.Any())
            return ApiResponse<WorkingTimeDto>.Error("No service IDs provided.");

        // Lấy WorkingTime của các service cần đặt
        var services = await _context.Service
            .Where(s => request.ServiceIds.Contains(s.Id))
            .Select(s => new { s.Id, s.WorkingTime })
            .ToListAsync(cancellationToken);

        if (!services.Any())
            return ApiResponse<WorkingTimeDto>.Error("No valid services found.");

        var totalDuration = services
            .Select(s => s.WorkingTime ?? TimeSpan.FromMinutes(60))
            .Aggregate(TimeSpan.Zero, (acc, cur) => acc + cur);

        // 2. Lấy lịch làm việc của technician
        var schedules = await _context.Calendar
            .Include(x => x.CalendarType)
            .Include(x => x.CalendarOverrides)
                .ThenInclude(x => x.CalendarType)
            .Where(ws => ws.TechnicianId == request.TechnicianId
                         && ws.StoreId == request.StoreId
                         && !ws.IsDeleted)
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

        if (offOverrides.Any() && !workingOverrides.Any())
            return ApiResponse<WorkingTimeDto>.Error("Technician is off on this day.");

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
            return ApiResponse<WorkingTimeDto>.Error("There are no schedules for this day.");

        // 3. Lấy danh sách booking đã có
        var bookedResponse = await _orderClient.GetBookedSlotsAsync(
            request.StoreId,
            request.TechnicianId,
            targetDate,
            cancellationToken);
        var bookedSlots = bookedResponse?.Data?.ToList() ?? new List<BookingTimeDto>();

        // 4. Tính thời lượng thực tế cho từng booking dựa trên ServiceIds của nó
        var allServiceIdsFromBookings = bookedSlots
            .SelectMany(b => b.ServiceIds ?? new List<int>())
            .Distinct()
            .ToList();

        var serviceDurationDict = new Dictionary<int, TimeSpan>();
        if (allServiceIdsFromBookings.Any())
        {
            var servicesFromDb = await _context.Service
                .Where(s => allServiceIdsFromBookings.Contains(s.Id))
                .Select(s => new { s.Id, s.WorkingTime })
                .ToListAsync(cancellationToken);

            serviceDurationDict = servicesFromDb.ToDictionary(
                s => s.Id,
                s => s.WorkingTime ?? TimeSpan.FromMinutes(60)
            );
        }
        var bookedIntervals = bookedSlots
        .Select(b =>
        {
            var duration = b.ServiceIds?
                .Select(id => serviceDurationDict.GetValueOrDefault(id, TimeSpan.FromMinutes(60)))
                .Aggregate(TimeSpan.Zero, (acc, cur) => acc + cur)
                ?? TimeSpan.FromMinutes(60);
            return new { Start = b.BookingTime, Duration = duration };
        })
        .ToList();
        foreach (var interval in bookedIntervals)
        {
            Console.WriteLine($"Booked: {interval.Start} - {interval.Start + interval.Duration}");
        }
        // 5. Sinh các slot khả dụng (step = 5 phút)çç
        var availableSlots = new List<string>();
        const int stepMinutes = 5;

        foreach (var time in workingTimes)
        {
            var start = time.WorkStartTime;
            var end = time.WorkEndTime - totalDuration; // phải đủ thời gian cho các dịch vụ cần đặt

            while (start <= end)
            {
                var slotEnd = start + totalDuration;

                // Kiểm tra chồng lấn với từng booking đã có (có tính gap)
                bool isOverlapping = bookedIntervals.Any(b =>
                    start < b.Start + b.Duration + Gap &&
                    slotEnd > b.Start
                );

                if (!isOverlapping)
                    availableSlots.Add(start.ToString(@"hh\:mm"));

                start = start.Add(TimeSpan.FromMinutes(stepMinutes));
            }
        }

        // 6. Trả về kết quả
        var earliestStart = workingTimes.Min(x => x.WorkStartTime);
        var latestEnd = workingTimes.Max(x => x.WorkEndTime);

        var result = new WorkingTimeDto
        {
            StoreId = request.StoreId,
            TechnicianId = request.TechnicianId,
            WorkDate = targetDate,
            WorkStartTime = earliestStart,
            WorkEndTime = latestEnd,
            AvailableSlots = availableSlots
        };

        return ApiResponse<WorkingTimeDto>.Success(result);
    }

    private static bool IsOff(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        name = name.Trim().ToLowerInvariant();
        return name.Contains("off") || name.Contains("absent");
    }
}