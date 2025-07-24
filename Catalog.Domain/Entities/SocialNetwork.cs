using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class SocialNetwork: BaseAuditableEntity<int>
{
    public string Name { get; set; } = null!;
    public string? Url { get; set; }
    public long StoreId { get; set; }
    public Store? Store { get; set; }
}
