using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Rewards.Models;

public class RewardDto: BaseAuditableDto<int>
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public RewardType? RewardType { get; set; }

    public ConversionType? ConversionType { get; set; }

    public double Point { get; set; }

    public decimal Cash { get; set; }

    public RewardStatus Status { get; set; }

    public int MerchantId { get; set; }

    public string? MerchantName { get; set; }

    private class Mapping : Profile
    {
        public Mapping ()
        {
            CreateMap<Reward, RewardDto>()
                .ForMember(dest => dest.MerchantName, opt => opt.MapFrom(src => src.Merchant.Name));
        }
    }
}

