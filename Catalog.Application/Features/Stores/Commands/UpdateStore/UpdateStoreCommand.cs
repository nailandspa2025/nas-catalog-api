using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Commands.UpdateStore;

public class UpdateStoreCommand: IRequest<ApiResponse<StoreDto>>
{
    public long Id { get; set; }

    public string StoreName { get; init; } = null!;

    public string? Avatar { get; init; }

    public string? AddressStore { get; init; }

    public int RatingStar { get; init; }

    public decimal Lat { get; init; }

    public decimal Lng { get; init; }

    public string? Hotline { get; init; }

    public TimeSpan OpenTime { get; init; }

    public TimeSpan CloseTime { get; init; }

    public string? GoogleReviewLink { get; set; }

    public string OwnerId { get; init; } = null!;

    public List<string> Images { get; init; } = new List<string>();
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
           .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if(entity == null)
        {
            throw new NotFoundException(nameof(Store), request.Id);
        }

        entity.StoreName = request.StoreName;
        entity.AddressStore = request.AddressStore;
        entity.RatingStar = request.RatingStar;
        entity.Lat = request.Lat;
        entity.Lng = request.Lng;
        entity.Hotline = request.Hotline;
        entity.OpenTime = request.OpenTime;
        entity.CloseTime = request.CloseTime;
        entity.GoogleReviewLink = request.GoogleReviewLink;
        entity.OwnerId = request.OwnerId;

        if (!string.IsNullOrEmpty(request.Avatar))
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
        return ApiResponse<StoreDto>.Success(_mapper.Map<StoreDto>(entity));
    }
}
