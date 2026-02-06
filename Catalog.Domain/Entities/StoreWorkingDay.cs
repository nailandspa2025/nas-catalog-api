using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class StoreWorkingDay : BaseAuditableEntity<int>
{
    public long StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public int DayOfWeek { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
}
 