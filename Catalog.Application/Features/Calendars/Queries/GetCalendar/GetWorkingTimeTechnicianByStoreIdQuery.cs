using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Calendars.Models;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Calendars.Queries.GetCalendar;

public record GetWorkingTimeTechnicianByStoreIdQuery : IRequest<ApiResponse<IEnumerable<WorkingTimeDto>>>
{
    public int StoreId { get; init; }

    public int TechnicianId { get; init; }

    public DateTime Date { get; init; }
}
public class GetWorkingTimeTechnicianByStoreIdQueryHandler : IRequestHandler<GetWorkingTimeTechnicianByStoreIdQuery, ApiResponse<IEnumerable<WorkingTimeDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetWorkingTimeTechnicianByStoreIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<WorkingTimeDto>>> Handle(GetWorkingTimeTechnicianByStoreIdQuery request, CancellationToken cancellationToken)
    {
        var targetDate = request.Date.Date;

        var schedules = await _context.Calendar
            .Include(x => x.CalendarOverrides)
            .Where(ws =>
                ws.TechnicianId == request.TechnicianId &&
                ws.StoreId == request.StoreId &&
                !ws.IsDeleted)
            .ToListAsync(cancellationToken);

        // Step 1: Check for overrides (ưu tiên nếu tồn tại)
        var overrideEntries = schedules
            .SelectMany(c => c.CalendarOverrides)
            .Where(o => o.WorkDate.Date == targetDate && !o.IsDeleted)
            .ToList();

        if (overrideEntries.Any())
        {
            var overrideDtos = overrideEntries.Select(o => new WorkingTimeDto
            {
                StoreId = request.StoreId,
                TechnicianId = request.TechnicianId,
                WorkDate = targetDate,
                WorkStartTime = o.WorkStartTime,
                WorkEndTime = o.WorkEndTime
            }).ToList();

            return ApiResponse<IEnumerable<WorkingTimeDto>>.Success(overrideDtos);
        }

        // Step 2: Lấy danh sách ngày đã được override (kể cả đã bị xóa)
        var overrideDates = schedules
            .SelectMany(c => c.CalendarOverrides)
            .Where(o => o.WorkDate.Date == targetDate)
            .Select(o => o.WorkDate.Date)
            .ToHashSet();

        // Step 3: Check lịch gốc có tính lặp
        var matchedSchedules = schedules
            .Where(ws =>
            {
                // Nếu ngày đã có override (dù đã xóa), thì không dùng lịch gốc
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

        if (matchedSchedules.Any())
        {
            var dtos = matchedSchedules.Select(ws => new WorkingTimeDto
            {
                StoreId = request.StoreId,
                TechnicianId = request.TechnicianId,
                WorkDate = targetDate,
                WorkStartTime = ws.WorkStartTime,
                WorkEndTime = ws.WorkEndTime
            }).ToList();

            return ApiResponse<IEnumerable<WorkingTimeDto>>.Success(dtos);
        }

        return ApiResponse<IEnumerable<WorkingTimeDto>>.Error("No working schedules found for the specified date.");
    }
}

