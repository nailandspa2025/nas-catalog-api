using AutoMapper;
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

    private class Mapping : Profile
    {
        public Mapping ()
        {
            CreateMap<ReviewStore, ReviewStoreDto>();
        }
    }
}
