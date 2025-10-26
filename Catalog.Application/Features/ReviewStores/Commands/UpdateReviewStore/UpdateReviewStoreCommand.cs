using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using BuildingBlocks.EventBus.Events;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ReviewStores.Models;
using Catalog.Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.ReviewStores.Commands.UpdateReviewStore;

public record UpdateReviewStoreCommand: IRequest<ApiResponse<ReviewStoreDto>>
{
    public int Id { get; init; }

    public int BookingId { get; init; }

    public long StoreId { get; init; }

    public int StoreRating { get; init; }

    public long TechnicianId { get; init; }

    public int TechnicianRating { get; init; }

    public int ServiceId { get; init; }

    public int ServiceRating { get; init; }

    public string? Content { get; init; }

    public bool IsActive { get; init; }
    public List<UpdateReviewTechnicianModel> ReviewTechnicians { get; init; } = new List<UpdateReviewTechnicianModel>();
    public List<UpdateReviewServiceModel> ReviewServices { get; init; } = new List<UpdateReviewServiceModel>();
    public List<IFormFile> Images { get; init; } = new List<IFormFile>();
    public List<string> LinkUrls { get; init; } = new List<string>();

}
public record UpdateReviewTechnicianModel
{
    public long TechnicianId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
}
public record UpdateReviewServiceModel
{
    public int ServiceId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
}
public class UpdateReviewStoreCommandHandler : IRequestHandler<UpdateReviewStoreCommand, ApiResponse<ReviewStoreDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IStorageService _storageService;

    public UpdateReviewStoreCommandHandler(ICatalogDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint, IStorageService storageService)
    {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _storageService = storageService;
    }


    public async Task<ApiResponse<ReviewStoreDto>> Handle(UpdateReviewStoreCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ReviewStore
            .Include(x => x.ReviewTechnicians)
            .Include(x => x.ReviewServices)
            .Include(x => x.ReviewFiles)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(ReviewStore), request.Id);
        }

        entity.BookingId = request.BookingId;
        entity.StoreId = request.StoreId;
        entity.StoreRating = request.StoreRating;
        entity.TechnicianId = request.TechnicianId;
        entity.TechnicianRating = request.TechnicianRating;
        entity.ServiceId = request.ServiceId;
        entity.StoreRating = request.StoreRating;
        entity.Content = request.Content;
        entity.IsActive = request.IsActive;
        entity.IsRated = true;

        var updatedImageUrls = new List<string>();
        var oldImageUrls = entity.ReviewFiles.Select(g => g.Url).Where(url => !request.LinkUrls.Contains(url)).ToList();
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
        entity.SetReviewFiles(
        updatedImageUrls.Select(url => new ReviewStoreFile
            {
                Url = url
            }).ToList()
        );

        if (request.ReviewTechnicians != null && request.ReviewTechnicians.Any())
        {
            var reviewTechnicians = new List<ReviewTechnician>();
            foreach (var item in request.ReviewTechnicians)
            {
                var reviewTechnician = new ReviewTechnician
                {
                    TechnicianId = item.TechnicianId,
                    Rating = item.Rating,
                    Comment = item.Comment,
                    IsRated = true
                };
                reviewTechnicians.Add(reviewTechnician);
            }
            entity.SetReviewTechnicians(reviewTechnicians);
        }
        if (request.ReviewServices != null && request.ReviewServices.Any())
        {
            var reviewServices = new List<ReviewService>();
            foreach (var item in request.ReviewServices)
            {
                var reviewService = new ReviewService
                {
                    ServiceId = item.ServiceId,
                    Rating = item.Rating,
                    Comment= item.Comment,
                    IsRated = true
                };
                reviewServices.Add(reviewService);
            }
            entity.SetReviewServices(reviewServices);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new BookingUpdateRateEvent
        {
            BookingId = entity.Id,
            IsRated = true
        });
        return ApiResponse<ReviewStoreDto>.Success(_mapper.Map<ReviewStoreDto>(entity));
    }
}
