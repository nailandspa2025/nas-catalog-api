using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Commands.CreateStore;
using Catalog.Application.Features.Stores.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Commands.UpdateStore;

public record UpdateStoreCommand : IRequest<ApiResponse<StoreDto>>
{
    public long Id { get; set; }

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

    public string? DeepLink { get; set; }

    public List<IFormFile> Images { get; init; } = new List<IFormFile>();

    public List<long> PrductIds { get; init; } = new List<long>();

    public List<string> LinkUrls { get; init; } = new List<string>();

    public bool IsAvatar { get; set; }

    public List<string> UserIds { get; init; } = new List<string>();

    public int? MerchantId { get; init; } = null;

    public int? BrandId { get; init; } = null;

    public string? Email { get; init; }

    public string? Description { get; init; }

    public int? ServicePackageId { get; init; } = null;
    public List<int> BankIds { get; init; } = new List<int>();
    public List<UpdateSocialNetworkModel> SocialNetworks { get; init; } = new List<UpdateSocialNetworkModel>();
    public UpdatePaypalModel? PaypalConfig { get; init; }

    public int Order { get; init; }
    
    public List<UpdateSoreWorkingDayModel> WorkingDays { get; init; } = new List<UpdateSoreWorkingDayModel>();
}
public record UpdateSocialNetworkModel
{
    public string Name { get; init; } = null!;
    public string? Url { get; init; }
    public SocialNetworkType Icon { get; init; }
}
public record UpdatePaypalModel
{
    public string ClientId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;
    public string Currency { get; init; } = "USD";
    public bool IsSandbox { get; init; }
}

public record UpdateSoreWorkingDayModel
{
    public int DayOfWeek { get; init; }
    public TimeSpan? OpenTime { get; init; }
    public TimeSpan? CloseTime { get; init; } 
}
public class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand, ApiResponse<StoreDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public UpdateStoreCommandHandler(ICatalogDbContext context, IMapper mapper, IStorageService storageService)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }

    public async Task<ApiResponse<StoreDto>> Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Store
           .Include(x => x.ImageGallerys)
           .Include(x => x.Products)
           .Include(x => x.BankAccounts)
           .Include(x => x.SocialNetworks)
           .Include(x => x.PayPalConfig)
           .Include(x => x.StoreWorkingDays)
           .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Store), request.Id);
        }
        var produts = await _context.Product.Where(x => request.PrductIds.Contains(x.Id)).ToListAsync(cancellationToken: cancellationToken);
        var banks = await _context.BankAccount.Where(x => request.BankIds.Contains(x.Id)).ToListAsync(cancellationToken: cancellationToken);
        entity.StoreName = request.StoreName;
        entity.AddressStore = request.AddressStore;
        entity.RatingStar = request.RatingStar;
        entity.Lat = request.Lat;
        entity.Lng = request.Lng;
        entity.Hotline = request.Hotline;
        entity.OpenTime = request.OpenTime;
        entity.CloseTime = request.CloseTime;
        entity.GoogleReviewLink = request.GoogleReviewLink;
        entity.MerchantId = request.MerchantId;
        entity.BrandId = request.BrandId;
        entity.Email = request.Email;
        entity.Description = request.Description;
        entity.DeepLink = request.DeepLink;
        entity.ServicePackageId = request.ServicePackageId;
        entity.Order = request.Order;

        if (request.Avatar != null && request.Avatar.Length > 0)
        {
            var imageUrl = await _storageService.SaveFileAsync(request.Avatar, cancellationToken);
            if (!string.IsNullOrEmpty(entity.Avatar))
                await _storageService.DeleteFileAsync(entity.Avatar, cancellationToken);
            entity.Avatar = imageUrl;
        }
        else if (request.IsAvatar)
        {
            if (!string.IsNullOrEmpty(entity.Avatar))
                await _storageService.DeleteFileAsync(entity.Avatar, cancellationToken);
            entity.Avatar = null;
        }

        var oldImageUrls = entity.ImageGallerys.Select(g => g.Url).Where(url => !request.LinkUrls.Contains(url)).ToList();
        if (oldImageUrls.Any())
        {
            await _storageService.DeleteFileAsync(oldImageUrls, cancellationToken);
        }
        var updatedImageUrls = new List<string>();
        {
            var imageUrls = await _storageService.SaveFilesAsync(request.Images, cancellationToken);
            updatedImageUrls.AddRange(imageUrls);
        }
        if (request.LinkUrls != null && request.LinkUrls.Any())
        {
            updatedImageUrls.AddRange(request.LinkUrls);
        }
        entity.SetImageGallerys(
        updatedImageUrls.Select(url => new StoreImageGallery
        {
            Url = url
        }).ToList()
        );

        entity.SetProducts(produts);
        entity.SetBanks(banks);
        var userStores = await _context.UserStore
        .Where(us => us.StoreId == entity.Id)
        .ToListAsync(cancellationToken);

        _context.UserStore.RemoveRange(userStores);
        if (request.UserIds != null && request.UserIds.Any())
        {
            var newUserStores = request.UserIds.Select(userId => new UserStore
            {
                UserId = userId,
                StoreId = entity.Id
            }).ToList();
            entity.SetStores(newUserStores);
        }
        var socialNetworks = new List<SocialNetwork>();
        if (request.SocialNetworks != null && request.SocialNetworks.Any())
        {
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
        }
        if (request.PaypalConfig != null)
        {
            entity.PayPalConfig ??= new PayPalConfig();

            entity.PayPalConfig.ClientId = request.PaypalConfig.ClientId;
            entity.PayPalConfig.ClientSecret = request.PaypalConfig.ClientSecret;
            entity.PayPalConfig.Currency = string.IsNullOrWhiteSpace(request.PaypalConfig.Currency)
                ? "USD"
                : request.PaypalConfig.Currency;
            entity.PayPalConfig.IsSandbox = request.PaypalConfig.IsSandbox;
        }
        else
        {
            entity.PayPalConfig = null;
        }

        entity.SetSocialNetworks(socialNetworks);
        var workingDays = new List<StoreWorkingDay>();
        if (request.WorkingDays != null && request.WorkingDays.Any())
        {
            foreach (var item in request.WorkingDays)
            {
                var workingDay = new StoreWorkingDay
                {
                    DayOfWeek = item.DayOfWeek,
                    OpenTime = item.OpenTime,
                    CloseTime = item.CloseTime,
                };
                workingDays.Add(workingDay);
            }
        }
        entity.SetStoreWorkingDays(workingDays);
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<StoreDto>.Success(_mapper.Map<StoreDto>(entity));
    }
}
