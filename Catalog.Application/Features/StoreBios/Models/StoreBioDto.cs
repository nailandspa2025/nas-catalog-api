using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.StoreBios.Models;

public class StoreBioDto: BaseAuditableDto
{
    public int Id { get; init; }

    public string? Text { get; set; }

    public string? File { get; set; }

    public string? Image { get; set; }

    public long StoreId { get; set; }

    public string ? StoreName { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<StoreBio, StoreBioDto>()
                .ForMember(des => des.StoreName, opt => opt.MapFrom(s => s.Store.StoreName));
        }

    }
}

