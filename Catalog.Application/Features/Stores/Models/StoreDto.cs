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

        public DateTime? OperatingHours { get; set; }

        public string? GoogleReviewLink { get; set; }

        public string OwnerId { get; set; } = null!;

        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Store, StoreDto>();
            }
        }
    }
}

