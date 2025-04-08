using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Banners.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Banners.Commands.UpdateBanner;

public record UpdateBannerCommand: IRequest<ApiResponse<BannerDto>>
{
    public int Id { get; init; }

    public string? Title { get; init; }

    public string? Link { get; init; }

    public bool IsActive { get; init; }

    public DateTime? ShowFrom { get; init; }

    public DateTime? ShowTo { get; init; }

    public List<IFormFile> Images { get; init; } = new List<IFormFile>();

    public List<string> LinkUrls { get; init; } = new List<string>();

}

public class UpdateBannerCommandHandler : IRequestHandler<UpdateBannerCommand, ApiResponse<BannerDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public UpdateBannerCommandHandler(ICatalogDbContext context, IMapper mapper, IStorageService storageService)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }

    public async Task<ApiResponse<BannerDto>> Handle(UpdateBannerCommand request, CancellationToken cancellationToken)
    {
        var updatedImageUrls = new List<string>();
        var entity = await _context.Banner
           .Include(x => x.ImageGallerys)
           .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Banner), request.Id);
        }
        entity.Title = request.Title;
        entity.Link = request.Link;
        entity.IsActive = request.IsActive;
        entity.ShowFrom = request.ShowFrom;
        entity.ShowTo = request.ShowTo;

        var oldImageUrls = entity.ImageGallerys.Select(g => g.Url).Where(url => !request.LinkUrls.Contains(url)).ToList();
        if (oldImageUrls.Any())
        {
            await _storageService.DeleteFileAsync(oldImageUrls, cancellationToken);
        }
        if (request.Images.Any())
        {
            var imageUrls = await _storageService.SaveFilesAsync(request.Images, cancellationToken);

            updatedImageUrls.AddRange(imageUrls);
        }
        if (request.LinkUrls != null && request.LinkUrls.Any())
        {
            updatedImageUrls.AddRange(request.LinkUrls);
        }
        entity.SetImageGallerys(
        updatedImageUrls.Select(url => new BannermageGaller
        {
            Url = url
            }).ToList()
        );

        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<BannerDto>.Success(_mapper.Map<BannerDto>(entity));
    }
}
