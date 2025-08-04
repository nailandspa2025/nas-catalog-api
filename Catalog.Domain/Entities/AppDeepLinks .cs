using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class AppDeepLink : BaseAuditableEntity<int>, ISoftDelete
{
    public string Code { get; set; } 
    public string Type { get; set; }
    public string TargetId { get; set; }
    public string IOSLink { get; set; }
    public string AndroidLink { get; set; }
    public string WebFallback { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? Deleted { get; set; }
    public bool IsDeleted { get; set; }
}
