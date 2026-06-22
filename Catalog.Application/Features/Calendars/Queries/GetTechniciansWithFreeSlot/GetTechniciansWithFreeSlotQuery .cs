using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.ApiClients.Clients.Identity.Models;
using BuildingBlocks.ApiClients.Clients.Order;
using BuildingBlocks.ApiClients.Clients.Order.Booking.Models; // ← Thêm
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Calendars.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Calendars.Queries.GetTechniciansWithFreeSlot;

public record GetTechniciansWithFreeSlotQuery : IRequest<ApiResponse<IEnumerable<TechnicianDto>>>
{
    public int StoreId { get; init; }
    public DateTime Date { get; init; }
    public TimeSpan Time { get; init; }
    // ❌ Bỏ ServiceIds
}

public class GetTechniciansWithFreeSlotQueryHandler : IRequestHandler<GetTechniciansWithFreeSlotQuery, ApiResponse<IEnumerable<TechnicianDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IOrderClient _orderClient;
    private readonly IIdentityClient _identityClient;

    private static readonly TimeSpan Gap = TimeSpan.FromMinutes(5); // Khoảng cách tối thiểu sau booking

    public GetTechniciansWithFreeSlotQueryHandler(ICatalogDbContext context, IMapper mapper, IOrderClient orderClient, IIdentityClient identityClient)
    {
        _context = context;
        _mapper = mapper;
        _orderClient = orderClient;
        _identityClient = identityClient;
    }

    public async Task<ApiResponse<IEnumerable<TechnicianDto>>> Handle(GetTechniciansWithFreeSlotQuery request, CancellationToken cancellationToken)
    {
        var techniciansResponse = await _identityClient.GetTechniciansByStoreIdAsync(request.StoreId, cancellationToken);
        var technicians = techniciansResponse?.Data;

        if (technicians == null || !technicians.Any())
            return ApiResponse<IEnumerable<TechnicianDto>>.Success(_mapper.Map<IEnumerable<TechnicianDto>>(new List<TechnicianDto>()));

        var availableTechnicians = new List<TechnicianDto>();

        foreach (var tech in technicians)
        {
            // 1. Lấy lịch làm việc
            var calendars = await _context.Calendar
                .Include(x => x.CalendarType)
                .Include(x => x.CalendarOverrides)
                    .ThenInclude(x => x.CalendarType)
                .Where(c => c.TechnicianId == tech.Id &&
                            c.StoreId == request.StoreId &&
                            !c.IsDeleted)
                .ToListAsync(cancellationToken);

            var workingTimes = CalculateWorkingTimes(calendars, request.Date, tech.Id, request.StoreId);

            // Kiểm tra technician có làm việc vào thời điểm request.Time không
            bool isWorking = workingTimes.Any(wt =>
                wt.WorkStartTime <= request.Time &&
                wt.WorkEndTime > request.Time
            );

            if (!isWorking) continue;

            // 2. Lấy danh sách booking của technician trong ngày
            var bookedResponse = await _orderClient.GetBookedSlotsAsync(
                request.StoreId,
                tech.Id,
                request.Date.Date,
                cancellationToken
            );
            var bookedSlots = bookedResponse?.Data?.ToList() ?? new List<BookingTimeDto>();

            // 3. Lấy WorkingTime cho tất cả ServiceId trong các booking
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

            // 4. Tính khoảng thời gian bị chiếm của từng booking
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

            // 5. Kiểm tra xem request.Time có nằm trong khoảng bị chiếm + Gap không
            bool isBooked = bookedIntervals.Any(interval =>
                request.Time >= interval.Start - Gap &&
                request.Time < interval.Start + interval.Duration + Gap
            );
            // 6. Nếu không bị chiếm, thêm technician vào danh sách
            if (!isBooked)
                availableTechnicians.Add(tech);
        }

        return ApiResponse<IEnumerable<TechnicianDto>>.Success(_mapper.Map<IEnumerable<TechnicianDto>>(availableTechnicians));
    }

    private List<WorkingTimeDto> CalculateWorkingTimes(List<Calendar> calendars, DateTime targetDate, long technicianId, int storeId)
    {
        var overrideEntries = calendars
            .SelectMany(c => c.CalendarOverrides)
            .Where(o => !o.IsDeleted && o.WorkDate.Date == targetDate)
            .ToList();

        var offOverrides = overrideEntries
            .Where(o => IsOff(o.CalendarType.Name))
            .ToList();

        var workingOverrides = overrideEntries
            .Where(o => !IsOff(o.CalendarType.Name))
            .ToList();

        if (offOverrides.Any() && !workingOverrides.Any())
            return new List<WorkingTimeDto>();

        if (workingOverrides.Any())
        {
            return workingOverrides.Select(o => new WorkingTimeDto
            {
                StoreId = storeId,
                TechnicianId = technicianId,
                WorkDate = targetDate,
                WorkStartTime = o.WorkStartTime,
                WorkEndTime = o.WorkEndTime
            }).ToList();
        }

        var overrideDates = calendars
            .SelectMany(c => c.CalendarOverrides)
            .Where(o => o.WorkDate.Date == targetDate)
            .Select(o => o.WorkDate.Date)
            .ToHashSet();

        var matched = calendars
            .Where(c =>
            {
                if (overrideDates.Contains(targetDate)) return false;
                if (c.Recurrence == null || c.Recurrence == RecurrenceType.None)
                    return c.WorkDate.Date == targetDate;
                if (c.WorkDate.Date > targetDate) return false;

                return c.Recurrence switch
                {
                    RecurrenceType.Daily => true,
                    RecurrenceType.Weekly => c.WorkDate.DayOfWeek == targetDate.DayOfWeek,
                    RecurrenceType.Monthly => c.WorkDate.Day == targetDate.Day,
                    RecurrenceType.Yearly => c.WorkDate.Day == targetDate.Day && c.WorkDate.Month == targetDate.Month,
                    _ => false
                };
            })
            .ToList();

        return matched.Select(ws => new WorkingTimeDto
        {
            StoreId = storeId,
            TechnicianId = technicianId,
            WorkDate = targetDate,
            WorkStartTime = ws.WorkStartTime,
            WorkEndTime = ws.WorkEndTime
        }).ToList();
    }

    private static bool IsOff(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        name = name.Trim().ToLowerInvariant();
        return name.Contains("off") || name.Contains("absent");
    }
}