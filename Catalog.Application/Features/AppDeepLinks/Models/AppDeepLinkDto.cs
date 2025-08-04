
using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.AppDeepLinks.Models;

public class AppDeepLinkDto: BaseAuditableDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Type { get; set; }
    public string TargetId { get; set; }
    public string IOSLink { get; set; }
    public string AndroidLink { get; set; }
    public string WebFallback { get; set; }
    public string ShortLink => $"https://deeplink.nasshine.com/{Code}";
    public string QrCodeUrl => $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=https://deeplink.nasshine.com/{Code}";
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AppDeepLink, AppDeepLinkDto>();
        }
    }
}
