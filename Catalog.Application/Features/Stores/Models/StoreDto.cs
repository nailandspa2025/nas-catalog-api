using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Stores.Models;

public class StoreDto: BaseAuditableDto
{
    public long Id { get; set; }

    public string StoreName { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? AddressStore { get; set; }

    public string? Email { get; set; }

    public string? Description { get; set; }

    public int RatingStar { get; set; }

    public double Lat { get; set; }

    public double Lng { get; set; }

    public string? Hotline { get; set; }

    public TimeSpan OpenTime { get; set; }

    public TimeSpan CloseTime { get; set; }

    public string? GoogleReviewLink { get; set; }

    public string OwnerId { get; set; } = null!;

    public List<long> ProductIds { get; set; } = new List<long>();

    public List<string> ProductsNames { get; set; } = new List<string>();

    public List<string> ImageUrls { get; set; } = new List<string>();

    public List<string> UserIds { get; set; } = new List<string>();

    public bool IsFavorite { get; set; }

    public int? MerchantId { get; set; } 

    public int? BrandId { get; set; }

    public string? DeepLink { get; set; }

    public int ServicePackageId { get; set; }
    public List<int> BankIds { get; set; } = new List<int>();
    public PayPalConfigDto ? PaypalConfig { get; set; }
    public List<SocialNetworkDto> SocialNetworks { get; set; } = new List<SocialNetworkDto>();
    public int Order { set; get; }
    public List<StoreWorkingDayDto> StoreWorkingDays { get; set; } = new List<StoreWorkingDayDto>();
    public List<PaymentProviderDto> PaymentProviders { get; set; }= new();
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Store, StoreDto>()
               .ForMember(dest => dest.ProductIds, opt => opt.MapFrom(src => src.Products.Select(p => p.Id).ToList()))
               .ForMember(dest => dest.ProductsNames, opt => opt.MapFrom(src => src.Products.Select(p => p.ProductName).ToList()))
               .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageGallerys.Select(i => i.Url).ToList()))
               .ForMember(dest => dest.UserIds, opt => opt.MapFrom(src => src.UserStores.Select(i => i.UserId).ToList()))
               .ForMember(dest => dest.BankIds, opt => opt.MapFrom(src => src.BankAccounts.Select(i => i.Id).ToList()));
        }
    }
}

public class SocialNetworkDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public SocialNetworkType Icon { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<SocialNetwork, SocialNetworkDto>();
        }
    }
}

public class PayPalConfigDto
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Currency { get; set; } = "USD";
    public bool IsSandbox { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PayPalConfig, PayPalConfigDto>();
        }
    }
}

public class PaymentProviderDto
{
    public int Id { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public bool IsActive { get; set; }

    public List<PaymentProviderSettingDto> Settings { get; set; }
        = new();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PaymentProvider, PaymentProviderDto>();
        }
    }
}
public class PaymentProviderSettingDto
{
    public int Id { get; set; }

    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PaymentProviderSetting, PaymentProviderSettingDto>();
        }
    }
}