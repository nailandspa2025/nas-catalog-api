using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class Banner: BaseAuditableEntity<int>, ISoftDelete
{

    public string ? Title { get; set; }

    public string ? Link { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ShowFrom { get; set; }

    public DateTime? ShowTo { get; set; }

    public string? DeletedBy { get ; set ; }

    public DateTime? Deleted { get ; set ; }

    public bool IsDeleted { get; set; }

    public virtual List<BannermageGaller> ImageGallerys { get; private set; } = new List<BannermageGaller>();

    public void SetImageGallerys(List<BannermageGaller> bannerImageGalleries)
    {
        this.ImageGallerys.Clear();
        this.ImageGallerys = bannerImageGalleries;
    }
}