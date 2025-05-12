using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Rewards.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Rewards.Queries.GetReward;

public record GetRewardByIdQuery : IRequest<ApiResponse<RewardDto>>
{
	public int Id { get; init; }
}

public class GetRewardByIdQueryhandler : IRequestHandler<GetRewardByIdQuery, ApiResponse<RewardDto>>
{
    private readonly ICatalogDbContext _contexxt;
    private readonly IMapper _mapper;

    public GetRewardByIdQueryhandler(ICatalogDbContext context, IMapper mapper)
    {
        _contexxt = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<RewardDto>> Handle(GetRewardByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _contexxt.Reward
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(Reward), request.Id);
        }

        return ApiResponse<RewardDto>.Success(_mapper.Map<RewardDto>(entity));
    }
}