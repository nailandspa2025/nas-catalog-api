using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Queries.GetStoreForMerchantsWithPagination;

public record GetStoreForMerchantsWithPaginationQuery: IRequest<ApiResponse<PaginatedList<StoreDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }
}

public class GetStoreMerchantsWithPaginationQueryHandler : IRequestHandler<GetStoreForMerchantsWithPaginationQuery, ApiResponse<PaginatedList<StoreDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public GetStoreMerchantsWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper, ICurrentUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<PaginatedList<StoreDto>>> Handle(GetStoreForMerchantsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();
        var query = _context.Store
            .Where(s => !s.IsDeleted &&
                        _context.UserStore.Any(us => us.UserId == _currentUser.UserId && us.StoreId == s.Id))
            .AsNoTracking();

        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(s => paramSearchText.Contains(s.StoreName));
        }

        var paginationResult = await query

            .OrderBy(x => x.Created)
            .ProjectTo<StoreDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<StoreDto>>.Success(paginationResult);

    }
}