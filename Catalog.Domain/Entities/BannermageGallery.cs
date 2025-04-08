using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class BannermageGaller: BaseEntity<int>
{
    public string? Url { get; set; }

    public int BannerId { get; set; }

    public Banner Banner { get; set; } = null!;
}

