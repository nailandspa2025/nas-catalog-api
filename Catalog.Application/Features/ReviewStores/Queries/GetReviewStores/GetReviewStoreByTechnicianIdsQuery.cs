using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.ApiClients.Clients.Identity.Models;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ReviewStores.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.ReviewStores.Queries.GetReviewStores;

public class GetReviewStoreByTechnicianIdsQuery : IRequest<ApiResponse<PaginatedList<ReviewTechnicianDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public int TechnicianId { get; init; }  
}

public class GetReviewStoreByTechnicianIdsQueryHandler : IRequestHandler<GetReviewStoreByTechnicianIdsQuery, ApiResponse<PaginatedList<ReviewTechnicianDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityClient _identityClient;

    public GetReviewStoreByTechnicianIdsQueryHandler(ICatalogDbContext context, IMapper mapper, IIdentityClient identityClient)
    {
        _context = context;
        _mapper = mapper;
        _identityClient = identityClient;
    }
    public async Task<ApiResponse<PaginatedList<ReviewTechnicianDto>>> Handle(GetReviewStoreByTechnicianIdsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ReviewTechnician
            .Where(s => s.TechnicianId == request.TechnicianId)
            .Include(s => s.ReviewStore)
            .ThenInclude(x => x.ReviewFiles)
            .AsNoTracking();

        var paginationResult = await query
           .OrderBy(x => x.Created)
           .ProjectTo<ReviewTechnicianDto>(_mapper.ConfigurationProvider)
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
        return ApiResponse<PaginatedList<ReviewTechnicianDto>>.Success(paginationResult);
    }
}
