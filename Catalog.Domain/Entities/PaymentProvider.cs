using BuildingBlocks.Persistence.Entities.Common;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities;

public class PaymentProvider : BaseAuditableEntity<int>
{
    public long StoreId { get; set; }

    public Store Store { get; set; } = null!;

    public PaymentMethod PaymentMethod { get; set; }

    public bool IsActive { get; set; }
    
    public ICollection<PaymentProviderSetting> Settings { get; set; }
        = new List<PaymentProviderSetting>();
}