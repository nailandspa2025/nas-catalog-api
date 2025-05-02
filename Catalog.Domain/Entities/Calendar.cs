using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities;

public class Calendar: BaseAuditableEntity<int>, ISoftDelete
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime WorkDate { get; set; } 

    public TimeSpan WorkStartTime { get; set; } 

    public TimeSpan WorkEndTime { get; set; }   

    public TimeSpan? BreakStartTime { get; set; } 

    public TimeSpan? BreakEndTime { get; set; } 

    public string? Location { get; set; }

    public CalendarStatus Status { get; set; } = CalendarStatus.Pending;

    public int? ReminderMinutesBefore { get; set; }

    public RecurrenceType? Recurrence { get; set; }

    public int? RecurrenceInterval { get; set; }

    public DateTime? RecurrenceEndDate { get; set; }

    public int CalendarTypeId { get; set; }

    public CalendarType CalendarType { get; set; } = null!;

    public long StoreId { get; set; }

    public Store? Store { get; set; }

    public long TechnicianId { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<CalendarOverride> CalendarOverrides { get; set; }
}

