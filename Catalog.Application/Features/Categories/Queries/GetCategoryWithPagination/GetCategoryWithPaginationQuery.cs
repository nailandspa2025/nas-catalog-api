using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Categories.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Categories.Queries.GetCategoryWithPagination;

public record GetCategoryWithPaginationQuery: IRequest<ApiResponse<PaginatedList<CategoryDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }
}

public class GetCategoryWithPaginationQueryHandler : IRequestHandler<GetCategoryWithPaginationQuery, ApiResponse<PaginatedList<CategoryDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetCategoryWithPaginationQueryHandler (
        ICatalogDbContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaginatedList<CategoryDto>>> Handle(GetCategoryWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();
        var query = _context.Category.Where(x => !x.IsDeleted).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(paramSearchText))
        {
            query = query.Where(s => s.Name.ToUpper().Contains(paramSearchText));
        }

        var paginationResult = await query
            .Include(x => x.Children)
            .Include(x => x.Services)
            .OrderBy(x => x.Created)
            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<CategoryDto>>.Success(paginationResult);
        
    }
}
