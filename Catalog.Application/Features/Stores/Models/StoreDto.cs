using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Stores.Models
{
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
        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Store, StoreDto>()
                   .ForMember(dest => dest.ProductIds, opt => opt.MapFrom(src => src.Products.Select(p => p.Id).ToList()))
                   .ForMember(dest => dest.ProductsNames, opt => opt.MapFrom(src => src.Products.Select(p => p.ProductName).ToList()))
                   .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageGallerys.Select(i => i.Url).ToList()))
                   .ForMember(dest => dest.UserIds, opt => opt.MapFrom(src => src.UserStores.Select(i => i.UserId).ToList()));
            }
        }
    }
}

