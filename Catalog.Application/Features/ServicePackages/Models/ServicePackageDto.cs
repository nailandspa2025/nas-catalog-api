using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Application.Features.Stores.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.ServicePackages.Models;

public class ServicePackageDto: BaseAuditableDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int DurationDays { get; set; }

    public List<int> ServiceIds { get; set; }
    
    public List<string> ServiceName { get; set; }
    private class Mapping : Profile 
    {
        public Mapping()
        {
            CreateMap<ServicePackage, ServicePackageDto>()
                .ForMember(dest => dest.ServiceIds, opt => opt.MapFrom(src => src.Services.Select(p => p.Id).ToList()))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Services.Select(p => p.Name).ToList()))
                ;
        }

    }
}
