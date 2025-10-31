using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.ApiClients.Clients.Identity.Models;
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
    private readonly IIdentityClient _identityClient;

    public GetReviewStoresWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper, IIdentityClient identityClient)
    {
        _context = context;
        _mapper = mapper;
        _identityClient = identityClient;
    }

    public async Task<ApiResponse<PaginatedList<ReviewStoreDto>>> Handle(GetReviewStoresWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();

        var query = _context.ReviewStore.Where(s => !s.IsDeleted).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            var lowerSearch = request.SearchText.ToLower();
            query = query.Where(s => s.Content.ToLower().Contains(lowerSearch));
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
            .Include(x => x.ReviewServices)
            .Include(x => x.ReviewTechnicians)
            .Include(x => x.ReviewFiles)
            .ProjectTo<ReviewStoreDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        try
        {
            var appAccountIds = paginationResult.Items.Select(s => s.AccountId).Distinct();
            if (appAccountIds.Any())
            {
                var appAccounts = (await _identityClient.GetAppAccountByIdsAsync(string.Join(",", appAccountIds), cancellationToken))?.Data;
                var appAccountDictionary = appAccounts?.ToDictionary(t => t.Id, t => t) ?? new Dictionary<int, AppAccountDto>();
                foreach (var reviews in paginationResult.Items)
                {
                    if (appAccountDictionary.TryGetValue(reviews.AccountId, out var account))
                    {
                        reviews.AccountInfo = account;
                    }
                }
            }
        }
        catch (Exception) { }
        return ApiResponse<PaginatedList<ReviewStoreDto>>.Success(paginationResult);

    }
}
