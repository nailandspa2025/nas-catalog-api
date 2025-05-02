using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.CalendarTypes.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.CalendarTypes.Queries.GetCalendarTypes;

public record GetCalendarTypeByIdsQuery: IRequest<ApiResponse<IEnumerable<CalendarTypeDto>>>
{
    public string Ids { get; init; } = null!;
}

public class GetCalendarTypeByIdsQueryHandler : IRequestHandler<GetCalendarTypeByIdsQuery, ApiResponse<IEnumerable<CalendarTypeDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetCalendarTypeByIdsQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<CalendarTypeDto>>> Handle(GetCalendarTypeByIdsQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids.Split(",");

        var stores = await _context.Product
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<CalendarTypeDto>>.Success(_mapper.Map<IEnumerable<CalendarTypeDto>>(stores));
    }
}