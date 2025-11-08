using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.StoreBios.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.StoreBios.Queries.GetStoreBioWithPagination;

public record GetStoreBioWithPaginationQuery: IRequest<ApiResponse<PaginatedList<StoreBioDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }
}

public class GetStoreBioWithPaginationQueryHandler : IRequestHandler<GetStoreBioWithPaginationQuery, ApiResponse<PaginatedList<StoreBioDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetStoreBioWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaginatedList<StoreBioDto>>> Handle(GetStoreBioWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();

        var query = _context.StoreBio.Where(x => !x.IsDeleted).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(s => s.Text.ToUpper().Contains(paramSearchText));
        }

        var paginationResult = await query
            
            .OrderBy(x => x.Created)
            .ProjectTo<StoreBioDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<StoreBioDto>>.Success(paginationResult);
    }
}
