using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Banners.Models;

public class BannerDto: BaseAuditableDto
{
    public int Id { get; set; }

    public List<string> ImageUrls { get; set; } = new List<string>();

    public string? Title { get; set; }

    public string? Link { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ShowFrom { get; set; }

    public DateTime? ShowTo { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Banner, BannerDto>()
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageGallerys.Select(i => i.Url).ToList())); ;
        }
    }
}

