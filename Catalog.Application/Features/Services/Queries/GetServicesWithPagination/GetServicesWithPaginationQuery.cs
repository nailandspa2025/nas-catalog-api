using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Services.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Services.Queries.GetServicesWithPagination;

public record GetServicesWithPaginationQuery : IRequest<ApiResponse<PaginatedList<ServiceDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }
}

public class GetServicesWithPaginationQueryHandler : IRequestHandler<GetServicesWithPaginationQuery, ApiResponse<PaginatedList<ServiceDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetServicesWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaginatedList<ServiceDto>>> Handle(GetServicesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();

        var query = _context.Service.AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(s => paramSearchText.Contains(s.Name));
        }
       
        var paginationResult = await query
            .OrderBy(x => x.Created)
            .ProjectTo<ServiceDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<ServiceDto>>.Success(paginationResult);

    }
}