using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Categories.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Categories.Queries.GetCategories;

public class GetCategoryByIdsQuery: IRequest<ApiResponse<IEnumerable<CategoryDto>>>
{
    public string Ids { get; init; } = null!;
}

public class GetCategoryByIdsQueryHandler : IRequestHandler<GetCategoryByIdsQuery, ApiResponse<IEnumerable<CategoryDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetCategoryByIdsQueryHandler (
        ICatalogDbContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<IEnumerable<CategoryDto>>> Handle(GetCategoryByIdsQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids.Split(",");

        var entities = await _context.Category
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<CategoryDto>>.Success(_mapper.Map<IEnumerable<CategoryDto>>(entities));
        
    }
}
