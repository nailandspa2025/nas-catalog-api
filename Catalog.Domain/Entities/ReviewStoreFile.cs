using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class ReviewStoreFile : BaseEntity<int>
{
    public string? Url { get; set; }

    public ReviewStore ReviewStore { get; set; } = null!;
}

