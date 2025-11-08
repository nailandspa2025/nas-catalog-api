using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.StoreBios.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.StoreBios.Queries.GetStoreBios;

public record GetStoreBioByIdsQuery: IRequest<ApiResponse<IEnumerable<StoreBioDto>>>
{
	public string Ids { get; init; }
}

public class GetStoreBioByIdsQueryHandler : IRequestHandler<GetStoreBioByIdsQuery, ApiResponse<IEnumerable<StoreBioDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetStoreBioByIdsQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<StoreBioDto>>> Handle(GetStoreBioByIdsQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids.Split(",");

        var stores = await _context.StoreBio
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<StoreBioDto>>.Success(_mapper.Map<IEnumerable<StoreBioDto>>(stores));
    }
}