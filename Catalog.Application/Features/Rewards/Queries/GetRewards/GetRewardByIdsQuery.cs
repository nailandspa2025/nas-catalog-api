using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Rewards.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Rewards.Queries.GetRewards;

public record GetRewardByIdsQuery: IRequest<ApiResponse<IEnumerable<RewardDto>>>
{
	public string Ids { get; init; } = null!;
}

public class GetRewardByIdsQueryHandler : IRequestHandler<GetRewardByIdsQuery, ApiResponse<IEnumerable<RewardDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetRewardByIdsQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<RewardDto>>> Handle(GetRewardByIdsQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids.Split(",");

        var rewards = await _context.Reward
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<RewardDto>>.Success(_mapper.Map<IEnumerable<RewardDto>>(rewards));
    }
}