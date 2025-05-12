using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Calendars.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Calendars.Commands.UpdateCaledar;

public record UpdateCalendarCommand: IRequest<ApiResponse<CalendarDto>>
{
    public int Id { get; init; }

    public string Title { get; init; } = null!;

    public string? Description { get; init; }

    public DateTime WorkDate { get; init; }

    public TimeSpan WorkStartTime { get; init; }

    public TimeSpan WorkEndTime { get; init; }

    public long StoreId { get; init; }

    public long TechnicianId { get; init; }

    public int CalendarTypeId { get; init; }

    public RecurrenceType? Recurrence { get; init; } = RecurrenceType.None;
}

public class UpdateCalendarCommandHandler : IRequestHandler<UpdateCalendarCommand, ApiResponse<CalendarDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public UpdateCalendarCommandHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CalendarDto>> Handle(UpdateCalendarCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Calendar
            .Include(x => x.CalendarOverrides)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Calendar), request.Id);
        }
        entity.Recurrence = request.Recurrence;
        if (request.Recurrence == RecurrenceType.None || request.Recurrence == null)
        {
            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.WorkDate = request.WorkDate.Date;
            entity.WorkStartTime = request.WorkStartTime;
            entity.WorkEndTime = request.WorkEndTime;
            entity.StoreId = request.StoreId;
            entity.TechnicianId = request.TechnicianId;
            entity.CalendarTypeId = request.CalendarTypeId;
            entity.CalendarOverrides?.ToList().ForEach(x => x.IsDeleted = false);
        }
        else
        {
            var overrideDate = request.WorkDate;
            var existingOverride = entity.CalendarOverrides?
    .           FirstOrDefault(x => x.WorkDate.Date == overrideDate.Date &&  x.CalendarId == entity.Id);
            if (existingOverride != null)
            {
                existingOverride.Title = request.Title;
                existingOverride.Description = request.Description;
                existingOverride.WorkStartTime = request.WorkStartTime;
                existingOverride.WorkEndTime = request.WorkEndTime;
                existingOverride.StoreId = request.StoreId;
                existingOverride.TechnicianId = request.TechnicianId;
                existingOverride.CalendarTypeId = request.CalendarTypeId;
            }
            else
            {
                var newOverride = new CalendarOverride
                {
                    CalendarId = entity.Id,
                    Title = request.Title,
                    Description = request.Description,
                    WorkDate = overrideDate,
                    WorkStartTime = request.WorkStartTime,
                    WorkEndTime = request.WorkEndTime,
                    StoreId = request.StoreId,
                    TechnicianId = request.TechnicianId,
                    CalendarTypeId = request.CalendarTypeId,
                    IsDeleted = false
                };

                await _context.CalendarOverride.AddAsync(newOverride, cancellationToken);
            }
        }
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<CalendarDto>.Success(_mapper.Map<CalendarDto>(entity));

    }
}
