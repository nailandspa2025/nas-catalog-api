using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Produts.Models;

public class ProductDto: BaseAuditableDto
{
	public long Id { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public long StoreId { get; set; }

    public string? StoreName { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Product, ProductDto>()
            .ForMember(x => x.StoreName, x => x.MapFrom(x => x.Store.StoreName));
        }
    }
}

