using BuildingBlocks.Persistence.Entities.Common;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities;
public class Service : BaseAuditableEntity<int>
{
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public string ? Description { get; set; }
    public string ? UrlImage { get; set; }

    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public int? Rating { get; set; }
    public TimeSpan ? WorkingTime { get; set; }
    public CurrencyCode Currency { get; set; }
    public virtual ICollection<ServicePackage> ServicePackages { get; set; } = new List<ServicePackage>();
}
