using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Queries.GetStores;

public record GetCategoryByIsdQuery: IRequest<ApiResponse<IEnumerable<StoreDto>>>
{
    public string Ids { get; init; } = null!;
}

public class GetStoreByIdsQueryHandler : IRequestHandler<GetCategoryByIsdQuery, ApiResponse<IEnumerable<StoreDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetStoreByIdsQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<StoreDto>>> Handle(GetCategoryByIsdQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids.Split(",");

        var stores = await _context.Store
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<StoreDto>>.Success(_mapper.Map<IEnumerable<StoreDto>>(stores));
    }
}