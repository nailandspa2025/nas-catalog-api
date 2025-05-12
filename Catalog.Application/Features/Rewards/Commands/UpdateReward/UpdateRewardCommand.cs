using System;
using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Rewards.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Rewards.Commands.UpdateReward;

public record UpdateRewardCommand: IRequest<ApiResponse<RewardDto>>
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public RewardType RewardType { get; init; }

    public ConversionType ConversionType { get; init; }

    public double Points { get; init; }

    public decimal Cash { get; init; }

    public RewardStatus Status { get; init; }

    public int MerchantId { get; init; }
}


public class UpdateRewardCommandHandler : IRequestHandler<UpdateRewardCommand, ApiResponse<RewardDto>>
{
    private readonly IMapper _mapper;
    private readonly ICatalogDbContext _context;

    public UpdateRewardCommandHandler(IMapper mapper, ICatalogDbContext context)
    {
        _context = context;
        _mapper = mapper;
    }

    public  async Task<ApiResponse<RewardDto>> Handle(UpdateRewardCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Reward.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Reward), request.Id);
        }
        entity.Name = request.Name;
        entity.RewardType = request.RewardType;

        return ApiResponse<RewardDto>.Success(_mapper.Map<RewardDto>(entity));
    }
}
