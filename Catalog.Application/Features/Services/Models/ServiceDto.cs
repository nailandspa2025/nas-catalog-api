
using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Services.Models;

public class ServiceDto : BaseAuditableDto<int>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string UrlImage { get; set; }
    private class Mapping : Profile
    {
        public Mapping ()
        {
            CreateMap<Service, ServiceDto>();
        }
    }
}
