using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Identity.Models;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.ReviewStores.Models;

public class ReviewStoreDto: BaseAuditableDto
{
	public int Id { get; set; }

    public int BookingId { get; set; }

    public long StoreId { get; set; }

    public int StoreRating { get; set; }

    public long TechnicianId { get; set; }

    public int TechnicianRating { get; set; }

    public int ServiceId { get; set; }

    public int ServiceRating { get; set; }

    public string? Content { get; set; }

    public bool IsActive { get; set; }

    public int  AccountId { get; set; }

    public AppAccountDto?  AccountInfo { get; set; }

    public bool IsRated { get; set; }

    public List<ReviewTechnicianDto> ReviewTechnicians { get; set; } = new List<ReviewTechnicianDto>();
    public List<ReviewServiceDto> ReviewServices { get; set; } = new List<ReviewServiceDto>();

    private class Mapping : Profile
    {
        public Mapping ()
        {
            CreateMap<ReviewStore, ReviewStoreDto>();
        }
    }
}
