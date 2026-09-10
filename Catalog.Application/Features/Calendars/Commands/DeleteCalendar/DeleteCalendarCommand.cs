using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Calendars.Commands.DeleteCalendar;

public record DeleteCalendarCommand : IRequest<ApiResponse>
{
    public int Id { get; init; }

    public DateTime WorkDate { get; init; }

    public CalendarScope Scope { get; init; } = CalendarScope.Single;
}

public class DeleteCalendarCommandHandler
    : IRequestHandler<DeleteCalendarCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public DeleteCalendarCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(
        DeleteCalendarCommand request,
        CancellationToken cancellationToken)
    {
        var calendar = await _context.Calendar
            .Include(x => x.CalendarOverrides)
            .Include(x => x.DaysOfWeek)
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (calendar == null)
        {
            throw new NotFoundException(
                nameof(Calendar),
                request.Id);
        }

        // ============================================
        // 1. Delete ALL
        // ============================================
        if (request.Scope == CalendarScope.All)
        {
            _context.Calendar.Remove(calendar);

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse.Success();
        }

        // ============================================
        // 2. Calendar không recurrence
        // ============================================
        if (calendar.Recurrence is null ||
            calendar.Recurrence == RecurrenceType.None)
        {
            _context.Calendar.Remove(calendar);

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse.Success();
        }

        // ============================================
        // 3. Delete SINGLE occurrence
        // ============================================
        var deleteDate = request.WorkDate.Date;

        deleteDate = DateTime.SpecifyKind(
            deleteDate,
            DateTimeKind.Utc);

        var existingOverride = calendar.CalendarOverrides
            .FirstOrDefault(x =>
                x.WorkDate.Date == deleteDate &&
                !x.IsDeleted);

        if (existingOverride != null)
        {
            existingOverride.IsDeleted = true;
        }
        else
        {
            var deletedOverride = new CalendarOverride
            {
                CalendarId = calendar.Id,

                Title = calendar.Title,
                Description = calendar.Description,

                WorkDate = deleteDate,

                WorkStartTime = calendar.WorkStartTime,
                WorkEndTime = calendar.WorkEndTime,

                StoreId = calendar.StoreId,
                TechnicianId = calendar.TechnicianId,
                CalendarTypeId = calendar.CalendarTypeId,

                IsDeleted = true
            };

            await _context.CalendarOverride.AddAsync(
                deletedOverride,
                cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}