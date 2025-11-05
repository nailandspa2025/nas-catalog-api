using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Categories.Models;

public class CategoryDto: BaseAuditableDto
{
    public int Id { get; set; }

	public string Name { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? ParentId { get; set; }

    public virtual ICollection<CategoryDto>? Children { get; set; }

    public int OrderNo { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public List<string> ServiceName { get; set; }

    public List<int> ServiceIds { get; set; }

    private class Mapping : Profile
	{
        public Mapping()
        {
            CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children.Where(c => !c.IsDeleted)))
            .ForMember(dest => dest.ServiceIds, opt => opt.MapFrom(src => src.Services.Select(s => s.Id).ToList()))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Services.Select(s => s.Name).ToList()));
        }
    }
}

