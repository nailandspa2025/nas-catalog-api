using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Stores.Models;

public class StoreWorkingDayDto : BaseAuditableDto
{
     public int Id { get; set; } 
    public int DayOfWeek { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<StoreWorkingDay, StoreWorkingDayDto>();
        }
    }
}