using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Produts.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Produts.Queries.GetProducts;

public class GetProductByIdsQuery: IRequest<ApiResponse<IEnumerable<ProductDto>>>
{
    public string Ids { get; init; } = null!;
}

public class GetProductByIdsQueryHandler : IRequestHandler<GetProductByIdsQuery, ApiResponse<IEnumerable<ProductDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetProductByIdsQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> Handle(GetProductByIdsQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids.Split(",");

        var stores = await _context.Product
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<ProductDto>>.Success(_mapper.Map<IEnumerable<ProductDto>>(stores));
    }
}