using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities;

public class Merchant: BaseAuditableEntity<int>, ISoftDelete
{
    public string Name { get; set; } = null!;

    public string? ShortName { get; set; }

    public string? TaxCode { get; set; }

    public string? ContractNumber { get; set; }

    public DateTime? ContractDate { get; set; }

    public DateTime? DeploymentDate { get; init; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public MerchantType Type { get; set; }

    public string? ZaloOA { get; set; }

    public string? Fanpage { get; set; }

    public string? Website { get; set; }

    public string? Address { get; set; }

    public string? Represent { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Logo { get; set; }

    public bool IsActive { get; set; }

    public string? ContactPhoneNumber { get; set; }

    public WeekdayOffMerchant WeekdayOff { get; set; }

    public virtual ICollection<MerchantContractImage> MerchantContractImages { get; set; } = new List<MerchantContractImage>();

    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();

    public int ServicePackageId { get; set; }

    public virtual ServicePackage? ServicePackage { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }


    public void SetContractImages(List<MerchantContractImage> contractImages)
    {
        this.MerchantContractImages.Clear();
        this.MerchantContractImages = contractImages;
    }

    public void SetBrands (List<Brand> brands)
    {
        this.Brands.Clear();
        this.Brands = brands;
    }
}

