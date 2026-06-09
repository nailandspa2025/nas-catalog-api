using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class PaymentProviderSetting : BaseAuditableEntity<int>
{
    public int PaymentProviderId { get; set; }
    public PaymentProvider PaymentProvider { get; set; } = null!;
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}
