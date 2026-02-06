using AutoMapper;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Common.Helpers;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Google.Rpc;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Commands.CreateStore;

public record CreateStoreCommand : IRequest<ApiResponse<StoreDto>>
{
    public string StoreName { get; init; } = null!;

    public IFormFile? Avatar { get; init; }

    public string? AddressStore { get; init; }

    public int RatingStar { get; init; }

    public double Lat { get; init; }

    public double Lng { get; init; }

    public string? Hotline { get; init; }

    public TimeSpan OpenTime { get; init; }

    public TimeSpan CloseTime { get; init; }

    public string? GoogleReviewLink { get; set; }

    //public string OwnerId { get; init; } = null!;

    public List<IFormFile> Images { get; init; } = new List<IFormFile>();

    public List<long> PrductIds { get; init; } = new List<long>();

    public List<string> UserIds { get; init; } = new List<string>();

    public int? MerchantId { get; init; } = null;

    public int? BrandId { get; init; } = null;

    public string? Email { get; init; }

    public string? Description { get; init; }

    public int? ServicePackageId { get; init; } = null;

    public List<int> BankIds { get; init; } = new List<int>();
    public List<CreateSocialNetworkModel> SocialNetworks { get; init; } = new List<CreateSocialNetworkModel>();

    public CreatePaypalModel? PaypalConfig { get; init; }
    public int Order { get; init; }
    public List<CreatesSoreWorkingDayModel> StoreWorkingDays { get; init; } = new List<CreatesSoreWorkingDayModel>();
}

public record CreateSocialNetworkModel
{
    public string Name { get; init; } = null!;
    public string? Url { get; init; }
    public SocialNetworkType Icon { get; init; }
}

public record CreatePaypalModel
{
    public string ClientId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;
    public string Currency { get; init; } = "USD";
    public bool IsSandbox { get; init; }
}

public record CreatesSoreWorkingDayModel
{
    public int DayOfWeek { get; init; }
    public TimeSpan? OpenTime { get; init; }
    public TimeSpan? CloseTime { get; init; } 
}
public class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, ApiResponse<StoreDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public CreateStoreCommandHandler(ICatalogDbContext context, IMapper mapper, IStorageService storageService)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }
    public async Task<ApiResponse<StoreDto>> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
    {
        var products = await _context.Product.Where(x => request.PrductIds.Contains(x.Id)).ToListAsync(cancellationToken: cancellationToken);
        var banks = await _context.BankAccount.Where(x => request.BankIds.Contains(x.Id)).ToListAsync(cancellationToken: cancellationToken); ;
        var code = StringGenerateRandom.Generate();
        var entity = new Store
        {
            StoreName = request.StoreName,
            AddressStore = request.AddressStore,
            RatingStar = request.RatingStar,
            Lat = request.Lat,
            Lng = request.Lng,
            Hotline = request.Hotline,
            OpenTime = request.OpenTime,
            CloseTime = request.CloseTime,
            GoogleReviewLink = request.GoogleReviewLink,
            MerchantId = request.MerchantId,
            BrandId = request.BrandId,
            Email = request.Email,
            Description = request.Description,
            ServicePackageId = request.ServicePackageId,
            DeepLink = $"http://deeplink.nasshine.com/{code}",
            Order = request.Order
        };
        if (request.Avatar != null && request.Avatar.Length > 0)
        {
            var imageUrl = await _storageService.SaveFileAsync(request.Avatar, cancellationToken);
            entity.Avatar = imageUrl;
        }
        if (request.Images.Any())
        {
            var imageUrls = await _storageService.SaveFilesAsync(request.Images, cancellationToken);
            var storeImages = imageUrls.Select(p => new StoreImageGallery
            {
                Url = p
            }).ToList();
            entity.SetImageGallerys(storeImages);
        }
        entity.SetProducts(products);
        entity.SetBanks(banks);
        if (request.UserIds != null && request.UserIds.Any())
        {
            var userStores = request.UserIds.Select(userId => new UserStore
            {
                UserId = userId,
                StoreId = entity.Id
            }).ToList();
            entity.SetStores(userStores);
        }
        if (request.SocialNetworks != null && request.SocialNetworks.Any())
        {
            var socialNetworks = new List<SocialNetwork>();
            foreach (var network in request.SocialNetworks)
            {
                var socialNetwork = new SocialNetwork
                {
                    Name = network.Name,
                    Url = network.Url,
                    Icon = network.Icon,
                };
                socialNetworks.Add(socialNetwork);
            }
            entity.SetSocialNetworks(socialNetworks);
        }
        if (request.PaypalConfig != null)
        {
            var paypalConfig = new PayPalConfig
            {
                ClientId = request.PaypalConfig.ClientId,
                ClientSecret = request.PaypalConfig.ClientSecret,
                Currency = request.PaypalConfig.Currency,
                IsSandbox = request.PaypalConfig.IsSandbox,
                StoreId = entity.Id,
            };
            entity.PayPalConfig = paypalConfig;
        }
        _context.Store.Add(entity);

        var deepLink = new AppDeepLink
        {
            Type = "store",
            TargetId = entity.Id.ToString(),
            Code = code,
            AndroidLink = $"intent://store-detail/{entity.Id}#Intent;package=com.nas.nas_mobile;scheme=nasshine;end",
            IOSLink = $"nasshine://store-detail/{entity.Id}",
            WebFallback = $"https://nasshine.com/{entity.Id}"
        };
        _context.AppDeepLink.Add(deepLink);

        if (request.StoreWorkingDays != null && request.StoreWorkingDays.Any())
        {
            var workingDays = request.StoreWorkingDays.Select(x => new StoreWorkingDay
            {
                StoreId = entity.Id,
                DayOfWeek = x.DayOfWeek,
                OpenTime = x.OpenTime,
                CloseTime =  x.CloseTime
            }).ToList();
            entity.SetStoreWorkingDays(workingDays);
        }
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<StoreDto>.Success(_mapper.Map<StoreDto>(entity));
    }
}
