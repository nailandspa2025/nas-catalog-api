using AutoMapper;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.CalendarTypes.Models;

public class CalendarTypeDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Color { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CalendarType, CalendarTypeDto>();
            
        }
    }
}

