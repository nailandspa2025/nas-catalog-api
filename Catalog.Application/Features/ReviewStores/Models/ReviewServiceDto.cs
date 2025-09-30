using AutoMapper;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.ReviewStores.Models;

public class ReviewServiceDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ReviewService, ReviewServiceDto>();
        }
    }
}
