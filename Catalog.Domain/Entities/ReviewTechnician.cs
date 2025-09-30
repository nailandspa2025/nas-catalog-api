using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class ReviewTechnician : BaseAuditableEntity<int>
{
    public long TechnicianId { get; set; }
    public int Rating { get; set; }
    public ReviewStore ReviewStore { get; set; }
    public string? Comment { get; set; }
}
