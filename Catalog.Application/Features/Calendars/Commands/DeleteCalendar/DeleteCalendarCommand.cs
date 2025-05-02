using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Calendars.Commands.DeleteCalendar;

public record DeleteCalendarCommand: IRequest<ApiResponse>
{
    public int Id { get; init; }

    public DateTime WorkDate { get; init; }
}

public class DeleteCalendarTypeCommandHandler : IRequestHandler<DeleteCalendarCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public DeleteCalendarTypeCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteCalendarCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Calendar
            .Include(x => x.CalendarOverrides)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Calendar), request.Id);
        }
        if (entity.Recurrence == RecurrenceType.None || entity.Recurrence == null)
        {
            _context.Calendar.Remove(entity);
        }
        else
        {
            var deleteDate = request.WorkDate;
            var existingOverride = entity.CalendarOverrides
           .FirstOrDefault(o => o.WorkDate.Date == deleteDate.Date && !o.IsDeleted);

            if (existingOverride != null)
            {
                existingOverride.IsDeleted = true;
            }
            else
            {
                var deletedOverride = new CalendarOverride
                {
                    CalendarId = entity.Id,
                    Title = entity.Title,
                    Description = entity.Description,
                    WorkDate = deleteDate,
                    WorkStartTime = entity.WorkStartTime,
                    WorkEndTime = entity.WorkEndTime,
                    StoreId = entity.StoreId,
                    TechnicianId = entity.TechnicianId,
                    CalendarTypeId = entity.CalendarTypeId,
                    IsDeleted = true
                };
                await _context.CalendarOverride.AddAsync(deletedOverride, cancellationToken);
            }
        }
           
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse.Success();
    }
}
