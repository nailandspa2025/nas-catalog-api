using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Calendars.Models;

public class CalendarDto: BaseAuditableDto
{
    //public int Id { get; set; }

    public Guid Id { get; set; } = Guid.NewGuid(); // Unique ID cho mỗi occurrence

    public int OriginalId { get; set; } // ID của lịch gốc

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime WorkDate { get; set; }

    public TimeSpan WorkStartTime { get; set; }

    public TimeSpan WorkEndTime { get; set; }

    public TimeSpan? BreakStartTime { get; set; }

    public TimeSpan? BreakEndTime { get; set; }

    public string? Location { get; set; }

    public CalendarStatus Status { get; set; } = CalendarStatus.Pending;

    public int? ReminderMinutesBefore { get; set; }

    public RecurrenceType? Recurrence { get; set; }

    public int? RecurrenceInterval { get; set; }

    public DateTime? RecurrenceEndDate { get; set; }

    public int CalendarTypeId { get; set; }

    //public CalendarType CalendarType { get; set; } = null!;

    public long StoreId { get; set; }

    //public Store? Store { get; set; }

    public long TechnicianId { get; set; }

    public string? Color { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Calendar, CalendarDto>()
                .ForMember(dest => dest.OriginalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.CalendarType.Color));
        }
    }
}

