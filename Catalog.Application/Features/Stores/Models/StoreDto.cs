using System;
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

        public int RatingStar { get; set; }

        public decimal Lat { get; set; }

        public decimal Lng { get; set; }

        public string? Hotline { get; set; }

<<<<<<< HEAD
        public DateTime? OperatingHours { get; set; }
=======
        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }
>>>>>>> 3399663 (product)

        public string? GoogleReviewLink { get; set; }

        public string OwnerId { get; set; } = null!;

<<<<<<< HEAD
=======
        public List<long> ProductIds { get; set; } = new List<long>();

        public List<string> ProductsNames { get; set; } = new List<string>();

>>>>>>> 3399663 (product)
        private class Mapping : Profile
        {
            public Mapping()
            {
<<<<<<< HEAD
                CreateMap<Store, StoreDto>();
=======
                CreateMap<Store, StoreDto>()
                   .ForMember(dest => dest.ProductIds, opt => opt.MapFrom(src => src.Products.Select(p => p.Id).ToList()))
                   .ForMember(dest => dest.ProductsNames, opt => opt.MapFrom(src => src.Products.Select(p => p.ProductName).ToList()));
>>>>>>> 3399663 (product)
            }
        }
    }
}

