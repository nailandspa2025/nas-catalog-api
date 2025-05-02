using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.CalendarTypes.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.CalendarTypes.Commands.UpdateCalendarType;

public record UpdateCalendarTypeCommand: IRequest<ApiResponse<CalendarTypeDto>>
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public string? Color { get; init; }
}
public class UpdateCalendarTypeCommandHandler : IRequestHandler<UpdateCalendarTypeCommand, ApiResponse<CalendarTypeDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public UpdateCalendarTypeCommandHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CalendarTypeDto>> Handle(UpdateCalendarTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CalendarType
             .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if(entity == null)
        {
            throw new NotFoundException(nameof(CalendarType), request.Id);
        }

        entity.Name = request.Name;
        entity.Color = request.Color;

        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<CalendarTypeDto>.Success(_mapper.Map<CalendarTypeDto>(entity));

    }
}
