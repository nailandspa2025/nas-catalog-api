using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Calendars.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;

namespace Catalog.Application.Features.Calendars.Commands.CreateCalendar;

public record CreateCalendarCommand: IRequest<ApiResponse<CalendarDto>>
{
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

public class CreateCalendarCommandHandler : IRequestHandler<CreateCalendarCommand, ApiResponse<CalendarDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public CreateCalendarCommandHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CalendarDto>> Handle(CreateCalendarCommand request, CancellationToken cancellationToken)
    {
        var entity = new Calendar
        {
            Title = request.Title,
            Description = request.Description,
            WorkDate = request.WorkDate,
            WorkStartTime = request.WorkStartTime,
            WorkEndTime = request.WorkEndTime,
            StoreId = request.StoreId,
            TechnicianId = request.TechnicianId,
            CalendarTypeId = request.CalendarTypeId,
            Recurrence = request.Recurrence
        };

        _context.Calendar.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<CalendarDto>.Success(_mapper.Map<CalendarDto>(entity));

    }
}