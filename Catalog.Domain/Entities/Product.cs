using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class Product : BaseAuditableEntity<long>, ISoftDelete
{

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public string? DeletedBy { get ; set; }

    public DateTime? Deleted { get ; set ; }

    public bool IsDeleted { get ; set; }

    public long? StoreId { get; set; }

    public virtual Store? Store { get; set; }
    public string ImageUrl { get; set; } = null!;

    //public virtual List<Store> Stores { get; private set; } = new List<Store>();

    //public void SetStores(List<Store> stores)
    //{
    //    this.Stores.Clear();
    //    this.Stores = stores;
    //}
}

