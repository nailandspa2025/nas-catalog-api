using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Catalog.Domain.Entities;

public class Store: BaseAuditableEntity<long> , ISoftDelete
{
    public string StoreName { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? AddressStore { get; set; }

    public int RatingStar { get; set; }

    public double Lat { get; set; }

    public double Lng { get; set; }

    public string?  Hotline { get; set; }

    public string? Email { get; set; }

    public string? Description { get; set; }

    public TimeSpan OpenTime { get; set; }

    public TimeSpan CloseTime { get; set; }

    public string? GoogleReviewLink { get; set; }

    public bool IsFavorite { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<StoreImageGallery> ImageGallerys { get; private set; } = new List<StoreImageGallery>();

    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    public virtual ICollection<UserStore> UserStores { get; set; } = new List<UserStore>();

    public ICollection<Calendar> Calendars { get; set; } = new List<Calendar>();

    public ICollection<CalendarOverride> CalendarOverrides { get; set; } = new List<CalendarOverride>();

    public int? MerchantId { get; set; }

    public Merchant? Merchant { get; set; }

    public int? BrandId { get; set; }

    public virtual ICollection<ReviewStore> ReviewStore { get; set; } = new List<ReviewStore>();

    public void SetImageGallerys(List<StoreImageGallery> storeImageGalleries)
    {
        this.ImageGallerys.Clear();
        this.ImageGallerys = storeImageGalleries;
    }

    public void SetProducts(List<Product> products)
    {
        this.Products.Clear();
        this.Products = products;
    }

    public void SetStores (List<UserStore> userStores)
    {
        this.UserStores.Clear();
        this.UserStores = userStores;
    }
}

