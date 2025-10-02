using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class ReviewService : BaseAuditableEntity<int>
{
    public ReviewStore ReviewStore { get; set; }
    public int ServiceId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsRated { get; set; }
}
