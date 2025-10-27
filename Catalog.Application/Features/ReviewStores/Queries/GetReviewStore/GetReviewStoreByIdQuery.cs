using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ReviewStores.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.ReviewStores.Queries.GetReviewStore;

public record GetReviewStoreByIdQuery: IRequest<ApiResponse<ReviewStoreDto>>
{
	public int Id { get; init; }
}

public class GetReviewStoreByIdQueryHandler : IRequestHandler<GetReviewStoreByIdQuery, ApiResponse<ReviewStoreDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetReviewStoreByIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<ReviewStoreDto>> Handle(GetReviewStoreByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.ReviewStore
            .Include(x => x.ReviewServices)
            .Include(x => x.ReviewTechnicians)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(ReviewStore), request.Id);
        }

        return ApiResponse<ReviewStoreDto>.Success(_mapper.Map<ReviewStoreDto>(entity));

    }
}