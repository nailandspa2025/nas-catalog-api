using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class CalendarOverride: BaseAuditableEntity<int>, ISoftDelete
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int CalendarId { get; set; }

    public DateTime WorkDate { get; set; }

    public TimeSpan WorkStartTime { get; set; }

    public TimeSpan WorkEndTime { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Calendar Calendar { get; set; }

    public long TechnicianId { get; set; }

    public long StoreId { get; set; }

    public Store? Store { get; set; }

    public int CalendarTypeId { get; set; }

    public CalendarType CalendarType { get; set; } = null!;
}

