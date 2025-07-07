using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;
public class Service : BaseAuditableEntity<int>
{
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public string ? Description { get; set; }
    public string ? UrlImage { get; set; }
    public virtual ICollection<ServicePackage> ServicePackages { get; set; } = new List<ServicePackage>();
}
