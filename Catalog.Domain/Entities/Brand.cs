using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class Brand: BaseAuditableEntity<int>, ISoftDelete
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Logo { get; set; }

    public int MerchantId { get; set; }

    public Merchant? Merchant { get; set; }

    public ICollection<Store> Stores { get; set; } = new List<Store>();

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }
}

			