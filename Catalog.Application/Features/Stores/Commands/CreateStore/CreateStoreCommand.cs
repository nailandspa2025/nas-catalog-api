using AutoMapper;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Commands.CreateStore;

public record CreateStoreCommand : IRequest<ApiResponse<StoreDto>>
{
    public string StoreName { get; init; } = null!;

    public IFormFile ? Avatar { get; init; }

    public string? AddressStore { get; init; }

    public int RatingStar { get; init; }

    public double Lat { get; init; }

    public double Lng { get; init; }

    public string? Hotline { get; init; }

    public TimeSpan  OpenTime { get; init; }

    public TimeSpan CloseTime { get; init; }

    public string? GoogleReviewLink { get; set; }

    //public string OwnerId { get; init; } = null!;

    public List<IFormFile> Images { get; init; } = new List<IFormFile>();

    public List<long> PrductIds { get; init; } = new List<long>();

    public List<string> UserIds { get; init; } = new List<string>();
}

public class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, ApiResponse<StoreDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public CreateStoreCommandHandler (ICatalogDbContext context, IMapper mapper, IStorageService storageService)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }
    public async Task<ApiResponse<StoreDto>> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
    {
        var productList = await _context.Product.Where(x => request.PrductIds.Contains(x.Id)).ToListAsync(cancellationToken: cancellationToken);
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
            //OwnerId = request.OwnerId

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
        entity.SetProducts(productList);
        if (request.UserIds != null && request.UserIds.Any())
        {
            var userStores = request.UserIds.Select(userId => new UserStore
            {
                UserId = userId,
                StoreId = entity.Id
            }).ToList();
            entity.SetStores(userStores);
        }
        _context.Store.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<StoreDto>.Success(_mapper.Map<StoreDto>(entity));
    }
}
