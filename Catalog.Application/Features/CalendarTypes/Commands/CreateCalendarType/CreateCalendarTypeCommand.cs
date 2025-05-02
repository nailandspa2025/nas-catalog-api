using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.CalendarTypes.Models;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.CalendarTypes.Commands.CreateCalendarType;

public record CreateCalendarTypeCommand : IRequest<ApiResponse<CalendarTypeDto>>
{
	public string Name { get; init; } = null!;

	public string? Color { get; init; }
}

public class CreateCalendarTypeCommandHandler : IRequestHandler<CreateCalendarTypeCommand, ApiResponse<CalendarTypeDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public CreateCalendarTypeCommandHandler (ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CalendarTypeDto>> Handle(CreateCalendarTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = new CalendarType
        {
            Name = request.Name,
            Color = request.Color
        };

        _context.CalendarType.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<CalendarTypeDto>.Success(_mapper.Map<CalendarTypeDto>(entity));

    }
}