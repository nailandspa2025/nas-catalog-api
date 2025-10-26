using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.UserStores.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.UserStores.Queries.GetUserStore;

public record GetUserIdsForCurrentUserStoreQuery: IRequest<ApiResponse<IEnumerable<UserStoreDto>>>
{
    public string UserId { get; init; } = null!;
}

public class GetUserIdsForCurrentUserStoreQueryHandler : IRequestHandler<GetUserIdsForCurrentUserStoreQuery, ApiResponse<IEnumerable<UserStoreDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetUserIdsForCurrentUserStoreQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<UserStoreDto>>> Handle(GetUserIdsForCurrentUserStoreQuery request, CancellationToken cancellationToken)
    {
        var storeIds = await _context.UserStore
            .Where(us => us.UserId == request.UserId)
            .Select(us => us.StoreId)
            .ToListAsync();

        if (!storeIds.Any())
            return ApiResponse<IEnumerable<UserStoreDto>>.Success(Enumerable.Empty<UserStoreDto>());

        var userIds = await _context.UserStore
            .Where(us => storeIds.Contains(us.StoreId))
            //.Select(us => new UserStoreDto { UserId = us.UserId })
            .Distinct()
            .ToListAsync();

        return ApiResponse<IEnumerable<UserStoreDto>>.Success(_mapper.Map<IEnumerable<UserStoreDto>>(userIds));
    }
}
