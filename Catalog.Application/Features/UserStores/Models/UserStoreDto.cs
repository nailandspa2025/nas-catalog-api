using AutoMapper;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.UserStores.Models;

public class UserStoreDto
{
    public long Id { get; set; }

    public string? UserId { get; set; }

    public long StoreId { get; set; }

    public string? BioText { get; set; }

    public string? BioFile { get; set; }

    public string? BioImage { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UserStore, UserStoreDto>()
                .ForMember(dest => dest.BioText,
                    opt => opt.MapFrom(src => src.Store != null ? src.Store.StoreBio!.Text : null))
                .ForMember(dest => dest.BioFile,
                    opt => opt.MapFrom(src => src.Store != null ? src.Store.StoreBio!.File : null))
                .ForMember(dest => dest.BioImage,
                    opt => opt.MapFrom(src => src.Store != null ? src.Store.StoreBio!.Image : null));
        }
    }
}

