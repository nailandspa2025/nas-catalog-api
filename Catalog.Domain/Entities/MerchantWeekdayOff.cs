using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class MerchantWeekdayOff: BaseEntity<int>
{
	public int WeekdayOff { get; set; }

	public int MerchantId { get; set; }

    public Merchant? Merchant { get; set; }

}

