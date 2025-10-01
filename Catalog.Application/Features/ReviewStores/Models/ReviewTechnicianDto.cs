using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Identity.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.ReviewStores.Models;

public class ReviewTechnicianDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public long TechnicianId { get; set; }
    public int AccountId { get; set; }
    public AppAccountDto? AccountInfo { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ReviewTechnician, ReviewTechnicianDto>()
                .ForMember(dest => dest.AccountId,
               opt => opt.MapFrom(src => src.ReviewStore.AccountId));
        }
    }
}
