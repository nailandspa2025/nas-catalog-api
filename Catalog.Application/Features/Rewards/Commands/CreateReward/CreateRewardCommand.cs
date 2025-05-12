using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Rewards.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
namespace Catalog.Application.Features.Rewards.Commands.CreateReward;

public record CreateRewardCommand: IRequest<ApiResponse<RewardDto>>
{
    public string Name { get; init; } = null!;

    public RewardType RewardType { get; init; }

    public ConversionType ConversionType { get; init; }

    public double Point { get; init; }

    public decimal Cash { get; init; }

    public RewardStatus Status { get; init; } = RewardStatus.Approved;

    public int MerchantId { get; init; }
}

public class CreateRewardCommandHandler : IRequestHandler<CreateRewardCommand, ApiResponse<RewardDto>>
{
    private readonly IMapper _mapper;
    private readonly ICatalogDbContext _context;

    public CreateRewardCommandHandler (IMapper mapper, ICatalogDbContext context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<RewardDto>> Handle(CreateRewardCommand request, CancellationToken cancellationToken)
    {
        var entity = new Reward
        {
            Name = request.Name,
            RewardType = request.RewardType,
            ConversionType = request.ConversionType,
            Point = request.Point,
            MerchantId = request.MerchantId,
            Status = request.Status
        };

        _context.Reward.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<RewardDto>.Success(_mapper.Map<RewardDto>(entity));
    }
}
