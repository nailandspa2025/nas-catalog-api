using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using Catalog.Application.Features.Stores.Queries.GetStoresWithPagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Queries.GetStoresWithPagination;

public class GetStoresWithPaginationQuery: IRequest<ApiResponse<PaginatedList<StoreDto>>>
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? SearchText { get; set; }
}

public class GetStoresWithPaginationQueryHandler : IRequestHandler<GetStoresWithPaginationQuery, ApiResponse<PaginatedList<StoreDto>>>
{
private readonly ICatalogDbContext _context;
private readonly IMapper _mapper;

public GetStoresWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper)
{
    _context = context;
    _mapper = mapper;
}

public async Task<ApiResponse<PaginatedList<StoreDto>>> Handle(GetStoresWithPaginationQuery request, CancellationToken cancellationToken)
{
    var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();

    var query = _context.Store.AsNoTracking();
    if (!paramSearchText.IsNullOrEmpty())
    {
        query = query.Where(s => paramSearchText.Contains(s.StoreName));
    }

    var paginationResult = await query
        .Where(x => !x.IsDeleted)
        .OrderBy(x => x.Created)
        .ProjectTo<StoreDto>(_mapper.ConfigurationProvider)
        .PaginatedListAsync(request.PageNumber, request.PageSize);

    return ApiResponse<PaginatedList<StoreDto>>.Success(paginationResult);
    
}
}