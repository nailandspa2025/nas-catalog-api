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

public record UpdateCalendarCommand : IRequest<ApiResponse<CalendarDto>>
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
    public DateTime? RecurrenceEndDate { get; init; }
    public List<DayOfWeek> DaysOfWeek { get; init; } = [];
    public int? RecurrenceInterval { get; init; }
    public CalendarScope Scope { get; init; } = CalendarScope.Single;

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
            .Include(x => x.DaysOfWeek)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Calendar), request.Id);
        }
        var workDate = DateTime.SpecifyKind(request.WorkDate.Date,DateTimeKind.Utc);

        if (request.Scope == CalendarScope.Single && entity.Recurrence != RecurrenceType.None && entity.Recurrence != null)
        {
            var overrideEntity = entity.CalendarOverrides
            .FirstOrDefault(x =>
                x.WorkDate.Date == workDate.Date &&
                !x.IsDeleted);

            if (overrideEntity == null)
            {
                overrideEntity = new CalendarOverride
                {
                    CalendarId = entity.Id,
                    WorkDate = workDate,

                    Title = request.Title,
                    Description = request.Description,

                    WorkStartTime = request.WorkStartTime,
                    WorkEndTime = request.WorkEndTime,

                    StoreId = request.StoreId,
                    TechnicianId = request.TechnicianId,
                    CalendarTypeId = request.CalendarTypeId,

                    IsDeleted = false
                };
                await _context.CalendarOverride.AddAsync(overrideEntity, cancellationToken);
            }
            else
            {
                overrideEntity.Title = request.Title;
                overrideEntity.Description = request.Description;
                overrideEntity.WorkStartTime = request.WorkStartTime;
                overrideEntity.WorkEndTime = request.WorkEndTime;
                overrideEntity.StoreId = request.StoreId;
                overrideEntity.TechnicianId = request.TechnicianId;
                overrideEntity.CalendarTypeId = request.CalendarTypeId;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return ApiResponse<CalendarDto>.Success(_mapper.Map<CalendarDto>(entity));
        }

        entity.Title = request.Title;
        entity.Description = request.Description;

        entity.WorkStartTime = request.WorkStartTime;
        entity.WorkEndTime = request.WorkEndTime;

        entity.StoreId = request.StoreId;
        entity.TechnicianId = request.TechnicianId;
        entity.CalendarTypeId = request.CalendarTypeId;
        entity.WorkDate = workDate;
        entity.Recurrence = request.Recurrence ?? RecurrenceType.None;
        entity.RecurrenceInterval = request.RecurrenceInterval ?? 1;
        entity.DaysOfWeek.Clear();

        if (entity.Recurrence == RecurrenceType.DayOfWeek)
        {
            foreach (var dayOfWeek in request.DaysOfWeek.Distinct())
            {
                entity.DaysOfWeek.Add(
                    new CalendarDayOfWeek
                    {
                        DayOfWeek = dayOfWeek
                    });
            }
        }
         await _context.SaveChangesAsync(cancellationToken);
         return ApiResponse<CalendarDto>.Success(_mapper.Map<CalendarDto>(entity));
    }
}
