using AutoMapper;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Stores.Models;

public class PaymentProviderSettingDto
{
    public int Id { get; set; }

    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PaymentProviderSetting, PaymentProviderSettingDto>();
        }
    }
}