using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Commands.UpdateStore;

public record UpdateStoreCommand: IRequest<ApiResponse<StoreDto>>
{
    public long Id { get; set; }

    public string StoreName { get; init; } = null!;

    public IFormFile ? Avatar { get; init; }

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

    public List<string> LinkUrls { get; init; } = new List<string>();

    public bool IsAvatar { get; set; }

    public List<string> UserIds { get; init; } = new List<string>();
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
           .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if(entity == null)
        {
            throw new NotFoundException(nameof(Store), request.Id);
        }
        var produtList = await _context.Product.Where(x => request.PrductIds.Contains(x.Id)).ToListAsync(cancellationToken:cancellationToken);
        entity.StoreName = request.StoreName;
        entity.AddressStore = request.AddressStore;
        entity.RatingStar = request.RatingStar;
        entity.Lat = request.Lat;
        entity.Lng = request.Lng;
        entity.Hotline = request.Hotline;
        entity.OpenTime = request.OpenTime;
        entity.CloseTime = request.CloseTime;
        entity.GoogleReviewLink = request.GoogleReviewLink;
        //entity.OwnerId = request.OwnerId;


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

        entity.SetProducts(produtList);
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

            _context.UserStore.AddRange(newUserStores);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<StoreDto>.Success(_mapper.Map<StoreDto>(entity));
    }
}
