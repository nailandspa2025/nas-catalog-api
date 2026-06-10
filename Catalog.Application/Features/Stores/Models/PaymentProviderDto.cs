using AutoMapper;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Stores.Models;
public class PaymentProviderDto
{
    public int Id { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public bool IsActive { get; set; }

    public List<PaymentProviderSettingDto> Settings { get; set; }
        = new();
    public string PaymentMethodName { get; set; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PaymentProvider, PaymentProviderDto>()
                .ForMember(
                    dest => dest.PaymentMethodName,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString())
                );
            ;
        }
    }
}
