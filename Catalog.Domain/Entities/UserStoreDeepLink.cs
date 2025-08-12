using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class UserStoreDeepLink
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public long StoreId { get; set; }
    public virtual Store Store { get; set; } = null!;
}