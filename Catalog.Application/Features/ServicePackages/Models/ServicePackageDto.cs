using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Application.Features.Stores.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.ServicePackages.Models;

public class ServicePackageDto: BaseAuditableDto
{
    private class Mapping : Profile 
    {
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int DurationDays { get; set; }
        public virtual ICollection<StoreDto> Stores { get; set; } = new List<StoreDto>();
        public Mapping()
        {
            CreateMap<ServicePackage, ServicePackageDto>();
        }
    }
}
