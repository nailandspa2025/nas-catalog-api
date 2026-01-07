
using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Services.Models;

public class ServiceDto : BaseAuditableDto<int>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string UrlImage { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public int? Rating { get; set; }
    public TimeSpan? WorkingTime { get; set; }
    public CurrencyCode Currency { get; set; }
    public List<int> CategoryIds { get; set; }
    public List<string> CategoryNames { get; set; }
    
    public List<int> StoreIds { get; set; } = new();   // ✅ thêm

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Service, ServiceDto>()
            .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.Categories.Select(c => c.Id).ToList()))
            .ForMember(dest => dest.CategoryNames, opt => opt.MapFrom(src => src.Categories.Select(c => c.Name).ToList()))
            .ForMember(
                dest => dest.StoreIds,
                opt => opt.MapFrom(src =>
                    src.ServicePackages
                        .SelectMany(sp => sp.Stores)
                        .Select(st => st.Id)
                        .Distinct()
                        .ToList()
                ));
        }
    }
}
