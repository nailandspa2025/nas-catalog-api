using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class CalendarType: BaseAuditableEntity<int>, ISoftDelete
{
    public string Name { get; set; } = null!;

    public string? Color { get; set; }

    public ICollection<Calendar> Calendars { get; set; } = new List<Calendar>();

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<CalendarOverride> CalendarOverrides { get; set; } = new List<CalendarOverride>();
}

