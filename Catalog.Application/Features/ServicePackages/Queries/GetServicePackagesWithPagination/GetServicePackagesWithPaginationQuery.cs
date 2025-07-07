using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ServicePackages.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.ServicePackages.Queries.GetServicePackagesWithPagination;

public record GetServicePackagesWithPaginationQuery: IRequest<ApiResponse<PaginatedList<ServicePackageDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }
}

public class GetRewardsWithPaginationQueryHandler : IRequestHandler<GetServicePackagesWithPaginationQuery, ApiResponse<PaginatedList<ServicePackageDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetRewardsWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaginatedList<ServicePackageDto>>> Handle(GetServicePackagesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();

        var query = _context.ServicePackage.Where(x => !x.IsDeleted).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(s => paramSearchText.Contains(s.Name));
        }
        
        var paginationResult = await query
            .Include(x => x.Services)
            .OrderBy(x => x.Created)
            .ProjectTo<ServicePackageDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<ServicePackageDto>>.Success(paginationResult);

    }
}