using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class ServicePackage: BaseAuditableEntity<int>, ISoftDelete
{
    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int DurationDays { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Merchant> Merchants { get; set; } = new List<Merchant>();
    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}

