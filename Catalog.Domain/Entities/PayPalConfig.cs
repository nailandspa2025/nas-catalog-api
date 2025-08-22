using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class PayPalConfig: BaseAuditableEntity<int>
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Currency { get; set; } = "USD";
    public bool IsSandbox { get; set; }
    public long StoreId { get; set; }
    public Store? Store { get; set; }
}
