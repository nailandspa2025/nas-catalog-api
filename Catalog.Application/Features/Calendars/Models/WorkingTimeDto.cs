using AutoMapper;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Calendars.Models;

public class WorkingTimeDto
{
    public long StoreId { get; set; }

    public long TechnicianId { get; set; }

    public DateTime WorkDate { get; set; }

    public TimeSpan WorkStartTime { get; set; }

    public TimeSpan WorkEndTime { get; set; }

    private class Mapping : Profile
    {
        public Mapping ()
        {
            CreateMap<Calendar, WorkingTimeDto>();
        }
    }
}