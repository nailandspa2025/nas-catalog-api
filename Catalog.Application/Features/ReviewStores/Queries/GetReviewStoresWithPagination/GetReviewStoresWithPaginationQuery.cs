using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ReviewStores.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.ReviewStores.Queries.GetReviewStoresWithPagination;

public record GetReviewStoresWithPaginationQuery: IRequest<ApiResponse<PaginatedList<ReviewStoreDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }

    public long ? StoreId { get; init; }

    public long? TechnicianId { get; init; }

    public bool ? IsActive { get; init; }
}

public class GetReviewStoresWithPaginationQueryHandler : IRequestHandler<GetReviewStoresWithPaginationQuery, ApiResponse<PaginatedList<ReviewStoreDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetReviewStoresWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaginatedList<ReviewStoreDto>>> Handle(GetReviewStoresWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();

        var query = _context.ReviewStore.Where(s => !s.IsDeleted).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(s => paramSearchText.Contains(s.Content));
        }
        if(request.StoreId.HasValue)
        {
            query = query.Where(s => s.StoreId == request.StoreId);
        }
        if (request.TechnicianId.HasValue)
        {
            query = query.Where(s => s.TechnicianId == request.TechnicianId);
        }
        if (request.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == request.IsActive.Value);
        }
        var paginationResult = await query
            .OrderBy(x => x.Created)
            .ProjectTo<ReviewStoreDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<ReviewStoreDto>>.Success(paginationResult);

    }
}
