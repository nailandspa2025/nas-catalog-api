using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class StoreImageGallery: BaseEntity<long>
{
    public string? Url { get; set; }

    public long StoreId { get; set; }

    public Store Store { get; set; } = null!;
}

