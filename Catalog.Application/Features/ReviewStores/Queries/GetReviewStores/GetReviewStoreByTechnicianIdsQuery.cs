using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ReviewStores.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.ReviewStores.Queries.GetReviewStores;

public class GetReviewStoreByTechnicianIdsQuery : IRequest<ApiResponse<PaginatedList<ReviewTechnicianDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public int TechnicianId { get; init; }
}

public class GetReviewStoreByTechnicianIdsQueryHandler : IRequestHandler<GetReviewStoreByTechnicianIdsQuery, ApiResponse<PaginatedList<ReviewTechnicianDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetReviewStoreByTechnicianIdsQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaginatedList<ReviewTechnicianDto>>> Handle(GetReviewStoreByTechnicianIdsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ReviewTechnician.Where(s => s.TechnicianId == request.TechnicianId).AsNoTracking();

        var paginationResult = await query
           .OrderBy(x => x.Created)
           .ProjectTo<ReviewTechnicianDto>(_mapper.ConfigurationProvider)
           .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<ReviewTechnicianDto>>.Success(paginationResult);
    }
}
