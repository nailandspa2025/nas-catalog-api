using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ReviewStores.Models;
using Catalog.Domain.Entities;
using MediatR;
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

    public UpdateReviewStoreCommandHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<ApiResponse<ReviewStoreDto>> Handle(UpdateReviewStoreCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ReviewStore
            .Include(x => x.ReviewTechnicians)
            .Include(x => x.ReviewServices)
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
        if(request.ReviewTechnicians != null && request.ReviewTechnicians.Any())
        {
            var reviewTechnicians = new List<ReviewTechnician>();
            foreach (var item in request.ReviewTechnicians)
            {
                var reviewTechnician = new ReviewTechnician
                {
                    TechnicianId = item.TechnicianId,
                    Rating = item.Rating,
                    Comment = item.Comment,
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
                };
                reviewServices.Add(reviewService);
            }
            entity.SetReviewServices(reviewServices);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<ReviewStoreDto>.Success(_mapper.Map<ReviewStoreDto>(entity));
    }
}
