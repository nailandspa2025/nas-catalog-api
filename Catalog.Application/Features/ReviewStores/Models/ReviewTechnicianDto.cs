using AutoMapper;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.ReviewStores.Models;

public class ReviewTechnicianDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public long TechnicianId { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ReviewTechnician, ReviewTechnicianDto>();
        }
    }
}
