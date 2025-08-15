using BuildingBlocks.Persistence.Entities.Common;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities;

public class SocialNetwork: BaseAuditableEntity<int>
{
    public string Name { get; set; } = null!;
    public string? Url { get; set; }
    public long StoreId { get; set; }
    public Store? Store { get; set; }
    public SocialNetworkType Icon { get; set; }
}
