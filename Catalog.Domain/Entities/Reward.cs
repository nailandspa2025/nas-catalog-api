using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities;

public class Reward: BaseAuditableEntity<int>, ISoftDelete
{
    public string Name { get; set; } = null!;

    public RewardType? RewardType { get; set; }

    public ConversionType? ConversionType { get; set; }

    public double Point { get; set; }

    public decimal Cash { get; set; }

    public RewardStatus Status { get; set; }

    public int? MerchantId { get; set; }

    public virtual Merchant? Merchant { get; set; }

    public string? DeletedBy { get; set ; }

    public DateTime? Deleted { get ;set; }

    public bool IsDeleted { get ; set ; }
}

