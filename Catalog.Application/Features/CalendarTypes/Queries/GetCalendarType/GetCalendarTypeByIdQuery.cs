using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.CalendarTypes.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.CalendarTypes.Queries.GetCalendarType;

public record GetCalendarTypeByIdQuery: IRequest<ApiResponse<CalendarTypeDto>>
{
    public int Id { get; set; }
}

public class GetCalendarTypeByIdQueryHandler : IRequestHandler<GetCalendarTypeByIdQuery, ApiResponse<CalendarTypeDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetCalendarTypeByIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CalendarTypeDto>> Handle(GetCalendarTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.CalendarType
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(CalendarType), request.Id);
        }

        return ApiResponse<CalendarTypeDto>.Success(_mapper.Map<CalendarTypeDto>(entity));
    }
}
