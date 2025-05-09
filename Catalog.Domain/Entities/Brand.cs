using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class Brand: BaseAuditableEntity<int>
{
    public string Name { get; set; } = null!;

    public string? Logo { get; set; }

    public int MerchantId { get; set; }

    public Merchant? Merchant { get; set; }

    //public ICollection<Store> Stores { get; set; } = new List<Store>();
}

			