using System;
using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Application.Features.Banners.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Categories.Models;

public class CategoryDto: BaseAuditableDto
{
    public int Id { get; set; }

	public string Name { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? ParentId { get; set; }

    public virtual ICollection<Category>? Children { get; set; }

    public int OrderNo { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    private class Mapping : Profile
	{
        public Mapping()
        {
            CreateMap<Category, CategoryDto>();
            
        }
    }
}

