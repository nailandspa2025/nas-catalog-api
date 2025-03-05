using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Application.Features.Stores.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Produts.Models;

public class ProductDto: BaseAuditableDto
{
	public long Id { get; set; }

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public virtual List<StoreDto> Stores { get; set; } = new List<StoreDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(x => x.Stores, x => x.MapFrom(a => a.Stores.Select(z => z.StoreName)));
                //.ForMember(x => x.CategoryName, x => x.MapFrom(x => x.Category.Name));
        }
    }
}

