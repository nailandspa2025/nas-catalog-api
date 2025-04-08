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
        if (request.Images.Any())
        {
            var imageUrls = await _storageService.SaveFilesAsync(request.Images, cancellationToken);
            var images = imageUrls.Select(p => new BannermageGaller
            {
                Url = p
            }).ToList();
            entity.SetImageGallerys(images);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<BannerDto>.Success(_mapper.Map<BannerDto>(entity));
    }
}
