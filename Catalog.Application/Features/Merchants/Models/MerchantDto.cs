using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Merchants.Models;

public class MerchantDto: BaseAuditableDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ShortName { get; set; }

    public string? TaxCode { get; set; }

    public string? ContractNumber { get; set; }

    public DateTime? ContractDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public MerchantType Type { get; set; }

    public string? ZaloOA { get; set; }

    public string? Fanpage { get; set; }

    public string? Website { get; set; }

    public string? Address { get; set; }

    public string? Represent { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Logo { get; set; }

    public bool IsActive { get; set; }

    public string? ContactPhoneNumber { get; set; }

    public List<int> WeekdayOffs { get; set; } = new List<int>();

    public List<string> ImageUrls { get; set; } = new List<string>();

    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Merchant, MerchantDto>()
                .ForMember(dest => dest.WeekdayOffs, opt => opt.MapFrom(src => src.MerchantWeekdayOffs.Select(i => i.WeekdayOff).ToList()))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.MerchantContractImages.Select(i => i.Url).ToList()))
                ;
        }
    }
}

