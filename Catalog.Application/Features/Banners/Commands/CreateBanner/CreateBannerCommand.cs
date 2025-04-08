using AutoMapper;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Banners.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Catalog.Application.Features.Banners.Commands.CreateBanner;

public record CreateBannerCommand:IRequest<ApiResponse<BannerDto>>
{

    public string? Title { get; init; }

    public string? Link { get; init; }

    public bool IsActive { get; init; }

    public DateTime? ShowFrom { get; init; }

    public DateTime? ShowTo { get; init; }

    public List<IFormFile> Images { get; init; } = new List<IFormFile>();
}

public class CreateBannerCommandHandler : IRequestHandler<CreateBannerCommand, ApiResponse<BannerDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public CreateBannerCommandHandler (ICatalogDbContext context, IMapper mapper, IStorageService storageService)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }

    public async Task<ApiResponse<BannerDto>> Handle(CreateBannerCommand request, CancellationToken cancellationToken)
    {
        var entity = new Banner
        {
            Title = request.Title,
            Link = request.Link,
            IsActive = request.IsActive,
            ShowFrom = request.ShowFrom,
            ShowTo = request.ShowTo,
        };
        if (request.Images.Any())
        {
            var imageUrls = await _storageService.SaveFilesAsync(request.Images, cancellationToken);
            var images = imageUrls.Select(p => new BannermageGaller
            {
                Url = p
            }).ToList();
            entity.SetImageGallerys(images);
        }
        
        _context.Banner.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<BannerDto>.Success(_mapper.Map<BannerDto>(entity));
    }
}