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

        var technicianIds = technicians.Select(t => t.Id).Distinct().ToList();

        // Cột WorkDate là timestamptz → tham số phải là UTC.
        var dayStart = DateTime.SpecifyKind(request.Date.Date, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);

        // 1. Lấy lịch của TẤT CẢ technician trong 1 query (trước đây: 1 query / technician).
        //    Chỉ cần các calendar bắt đầu trước hết ngày đó, hoặc có override trong ngày đó,
        //    và chỉ cần override của đúng ngày đó — CalculateWorkingTimes không dùng ngày khác.
        var calendars = await _context.Calendar
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.CalendarType)
            .Include(x => x.CalendarOverrides.Where(o => o.WorkDate >= dayStart && o.WorkDate < dayEnd))
                .ThenInclude(x => x.CalendarType)
            .Where(c => technicianIds.Contains(c.TechnicianId) &&
                        c.StoreId == request.StoreId &&
                        !c.IsDeleted &&
                        (c.WorkDate < dayEnd ||
                         c.CalendarOverrides.Any(o => o.WorkDate >= dayStart && o.WorkDate < dayEnd)))
            .ToListAsync(cancellationToken);

        var calendarsByTechnician = calendars.ToLookup(c => c.TechnicianId);

        // Technician đang làm việc tại thời điểm request.Time
        var workingTechnicians = technicians
            .Where(tech => CalculateWorkingTimes(calendarsByTechnician[tech.Id].ToList(), request.Date, tech.Id, request.StoreId)
                .Any(wt => wt.WorkStartTime <= request.Time && wt.WorkEndTime > request.Time))
            .ToList();

        if (!workingTechnicians.Any())
            return ApiResponse<IEnumerable<TechnicianDto>>.Success(_mapper.Map<IEnumerable<TechnicianDto>>(new List<TechnicianDto>()));

        // 2. Lấy booking của các technician song song (trước đây: tuần tự từng người).
        var bookedSlotsPerTechnician = await Task.WhenAll(workingTechnicians.Select(async tech =>
        {
            var bookedResponse = await _orderClient.GetBookedSlotsAsync(
                request.StoreId,
                tech.Id,
                request.Date.Date,
                cancellationToken);

            return (Technician: tech, BookedSlots: bookedResponse?.Data?.ToList() ?? new List<BookingTimeDto>());
        }));

        // 3. Lấy WorkingTime cho tất cả ServiceId của mọi booking trong 1 query.
        var allServiceIdsFromBookings = bookedSlotsPerTechnician
            .SelectMany(x => x.BookedSlots)
            .SelectMany(b => b.ServiceIds ?? new List<int>())
            .Distinct()
            .ToList();

        var serviceDurationDict = new Dictionary<int, TimeSpan>();
        if (allServiceIdsFromBookings.Any())
        {
            serviceDurationDict = await _context.Service
                .AsNoTracking()
                .Where(s => allServiceIdsFromBookings.Contains(s.Id))
                .Select(s => new { s.Id, s.WorkingTime })
                .ToDictionaryAsync(
                    s => s.Id,
                    s => s.WorkingTime ?? TimeSpan.FromMinutes(60),
                    cancellationToken);
        }

        // 4–6. Technician rảnh nếu request.Time không nằm trong khoảng bị chiếm (+ Gap) của booking nào.
        var availableTechnicians = bookedSlotsPerTechnician
            .Where(x => !x.BookedSlots.Any(b =>
            {
                var duration = b.ServiceIds?
                    .Select(id => serviceDurationDict.GetValueOrDefault(id, TimeSpan.FromMinutes(60)))
                    .Aggregate(TimeSpan.Zero, (acc, cur) => acc + cur)
                    ?? TimeSpan.FromMinutes(60);

                return request.Time >= b.BookingTime - Gap &&
                       request.Time < b.BookingTime + duration + Gap;
            }))
            .Select(x => x.Technician)
            .ToList();

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