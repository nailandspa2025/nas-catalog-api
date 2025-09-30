using AutoMapper;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ReviewStores.Models;
using Catalog.Application.Features.Stores.Commands.UpdateStore;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.ReviewStores.Commands.CreateReviewStore;

public record CreateReviewStoreCommand: IRequest<ApiResponse<ReviewStoreDto>>
{
    public int BookingId { get; init; }

    public long StoreId { get; init; }

    public int StoreRating { get; init; }

    public long TechnicianId { get; init; }

    public int TechnicianRating { get; init; }

    public int ServiceId { get; init; }

    public int ServiceRating { get; init; }

    public string? Content { get; init; }

    public bool IsActive { get; init; }

    public List<CreateReviewTechnicianModel> ReviewTechnicians { get; init; } = new List<CreateReviewTechnicianModel>();
    public List<CreateReviewServiceModel> ReviewServices { get; init; } = new List<CreateReviewServiceModel>();
}
public record CreateReviewTechnicianModel
{
    public long TechnicianId { get; init; }
    public int Rating { get; init; }
    public string ? Comment { get; init; }
}
public record CreateReviewServiceModel
{
    public int ServiceId { get; init; } 
    public int Rating { get; init; }
    public string ? Comment { get; init; }
}

public class CreateReviewStoreCommandHandler : IRequestHandler<CreateReviewStoreCommand, ApiResponse<ReviewStoreDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public CreateReviewStoreCommandHandler (ICatalogDbContext context, IMapper mapper, ICurrentUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<ReviewStoreDto>> Handle(CreateReviewStoreCommand request, CancellationToken cancellationToken)
    {
        var entity = new ReviewStore
        {
            BookingId = request.BookingId,
            StoreId = request.StoreId,
            StoreRating = request.StoreRating,
            TechnicianId = request.TechnicianId,
            TechnicianRating = request.TechnicianRating,
            ServiceId = request.ServiceId,
            ServiceRating = request.ServiceRating,
            Content = request.Content,
            IsActive = request.IsActive,
            AccountId = int.TryParse(_currentUser.UserId, out var id) ? id : 0,
        };
        if (request.ReviewTechnicians != null && request.ReviewTechnicians.Any())
        {
            var reviewTechnicians = new List<ReviewTechnician>();
            foreach (var item in request.ReviewTechnicians)
            {
                var reviewTechnician = new ReviewTechnician
                {
                    TechnicianId = item.TechnicianId,
                    Rating = item.Rating,
                    Comment= item.Comment,
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
                    Comment = item.Comment,
                };
                reviewServices.Add(reviewService);
            }
            entity.SetReviewServices(reviewServices);
        }
        _context.ReviewStore.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<ReviewStoreDto>.Success(_mapper.Map<ReviewStoreDto>(entity));
    }
}

