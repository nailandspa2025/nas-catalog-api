using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Rewards.Models;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Rewards.Queries.GetRewardsWithPagination;

public record GetRewardsWithPaginationQuery: IRequest<ApiResponse<PaginatedList<RewardDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }

    public int? MerchantId { get; init; }

    public RewardStatus? Status { get; init; }
}

public class GetRewardsWithPaginationQueryHandler : IRequestHandler<GetRewardsWithPaginationQuery, ApiResponse<PaginatedList<RewardDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetRewardsWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaginatedList<RewardDto>>> Handle(GetRewardsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();

        var query = _context.Reward.Where(x => !x.IsDeleted).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(s => s.Name.ToUpper().Contains(paramSearchText));

        }
        if (request.MerchantId.HasValue)
        {
            query = query.Where(x => x.MerchantId == request.MerchantId.Value);
        }
        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }
        var paginationResult = await query
            .OrderBy(x => x.Created)
            .ProjectTo<RewardDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<RewardDto>>.Success(paginationResult);

    }
}