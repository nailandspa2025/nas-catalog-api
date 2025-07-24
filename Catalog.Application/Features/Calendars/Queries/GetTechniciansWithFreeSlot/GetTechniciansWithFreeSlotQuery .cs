using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.ApiClients.Clients.Identity.Models;
using BuildingBlocks.ApiClients.Clients.Order;
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
}

public class GetTechniciansWithFreeSlotQueryHandler: IRequestHandler<GetTechniciansWithFreeSlotQuery, ApiResponse<IEnumerable<TechnicianDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IOrderClient _orderClient;
    private readonly IIdentityClient _identityClient;

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
        {
            return ApiResponse<IEnumerable<TechnicianDto>>.Success(_mapper.Map<IEnumerable<TechnicianDto>>(new List<TechnicianDto>()));
        }

        var availableTechnicians = new List<TechnicianDto>();

        foreach (var tech in technicians)
        {
            var calendars = await _context.Calendar
                .Include(x => x.CalendarOverrides)
                .Where(c =>
                    c.TechnicianId == tech.Id &&
                    c.StoreId == request.StoreId &&
                    !c.IsDeleted)
                .ToListAsync(cancellationToken);

            var workingTimes = CalculateWorkingTimes(calendars, request.Date, tech.Id, request.StoreId);

            var isAvailable = workingTimes.Any(wt =>
                wt.WorkStartTime <= request.Time &&
                wt.WorkEndTime > request.Time);

            //  Nếu KHÔNG làm việc vào thời điểm đó => bỏ qua
            if (!isAvailable) continue;

            // ✅ Có ca làm => kiểm tra có bị đặt lịch chưa
            var booked = await _orderClient.GetBookedSlotsAsync(
                request.StoreId,
                tech.Id,
                request.Date.Date,
                cancellationToken
            );

            var isBooked = booked?.Data?.Any(b =>
                 request.Time >= b.BookingTime &&
                 request.Time < b.BookingTime.Add(TimeSpan.FromMinutes(60))
             ) ?? false;

            // Nếu chưa bị đặt => thêm vào danh sách
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

        if (overrideEntries.Any())
        {
            return overrideEntries.Select(o => new WorkingTimeDto
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
}
