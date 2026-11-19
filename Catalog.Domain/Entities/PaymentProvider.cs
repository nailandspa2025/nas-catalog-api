using BuildingBlocks.Persistence.Entities.Common;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities;

public class PaymentProvider : BaseAuditableEntity<int>
{
    public long StoreId { get; set; }

    public Store Store { get; set; } = null!;

    public PaymentMethod PaymentMethod { get; set; }

    public bool IsActive { get; set; }

    // Phí cà thẻ (surcharge) cộng vào hoá đơn khi khách thanh toán bằng phương thức này:
    // surcharge = (tiền dịch vụ - giảm giá) * SurchargePercent / 100 + SurchargeFixedAmount
    public decimal SurchargePercent { get; set; }

    public decimal SurchargeFixedAmount { get; set; }
    
    public ICollection<PaymentProviderSetting> Settings { get; set; }
        = new List<PaymentProviderSetting>();
}