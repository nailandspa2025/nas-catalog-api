using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class MerchantContractImage: BaseEntity<int>
{
    public string? Url { get; set; }

    public int MerchantId { get; set; }

    public Merchant? Merchant { get; set; }
}

