using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Queries.GetStoreForMobileWithPagination;

public record GetStoreForMobileWithPaginationQuery: IRequest<ApiResponse<PaginatedList<StoreDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }

    public int? Rating { get; init; }
}

public class GetStoreForMobileWithPaginationQueryHandler : IRequestHandler<GetStoreForMobileWithPaginationQuery, ApiResponse<PaginatedList<StoreDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public GetStoreForMobileWithPaginationQueryHandler (ICatalogDbContext context, IMapper mapper, ICurrentUser currentUser)
    {
        _context = context;
        _mapper = mapper;  
        _currentUser = currentUser;
    }
    public async Task<ApiResponse<PaginatedList<StoreDto>>> Handle(GetStoreForMobileWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();
        var query = _context.Store
            .Where(s => !s.IsDeleted &&
            _context.UserStoreDeepLink.Any(usd => usd.UserId == _currentUser.UserId && usd.StoreId == s.Id))
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(paramSearchText))
        {
            query = query.Where(s => s.StoreName.ToUpper().Contains(paramSearchText)
            || s.AddressStore.ToUpper().Contains(paramSearchText));
        }
        if (request.Rating.HasValue)
        {
            query = query.Where(s => s.RatingStar == request.Rating.Value);
        }
        var paginationResult = await query
            .OrderBy(x => x.Created)
            .ProjectTo<StoreDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<StoreDto>>.Success(paginationResult);
    }
}