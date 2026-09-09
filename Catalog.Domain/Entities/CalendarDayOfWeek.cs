using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class CalendarDayOfWeek: BaseAuditableEntity
{
    public int CalendarId { get; set; }

    public Calendar Calendar { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }
}