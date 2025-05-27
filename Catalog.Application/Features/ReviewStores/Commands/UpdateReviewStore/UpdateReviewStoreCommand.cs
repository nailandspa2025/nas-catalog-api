using AutoMapper;
using BuildingBlocks.Authentication.Abstractions;
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

        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<ReviewStoreDto>.Success(_mapper.Map<ReviewStoreDto>(entity));
    }
}
