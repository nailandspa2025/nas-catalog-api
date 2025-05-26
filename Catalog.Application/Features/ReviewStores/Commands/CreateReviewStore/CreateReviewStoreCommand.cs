using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ReviewStores.Models;
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
}

public class CreateReviewStoreCommandHandler : IRequestHandler<CreateReviewStoreCommand, ApiResponse<ReviewStoreDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public CreateReviewStoreCommandHandler (ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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
            IsActive = request.IsActive
        };
        _context.ReviewStore.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<ReviewStoreDto>.Success(_mapper.Map<ReviewStoreDto>(entity));
    }
}

