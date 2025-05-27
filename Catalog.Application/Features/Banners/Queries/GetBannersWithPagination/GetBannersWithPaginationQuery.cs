using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Banners.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Banners.Queries.GetBannersWithPagination;

public record GetBannersWithPaginationQuery : IRequest<ApiResponse<PaginatedList<BannerDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }

    public bool IsMobile { get; set; } = false;
}

public class GetBannersWithPaginationQueryHandler : IRequestHandler<GetBannersWithPaginationQuery, ApiResponse<PaginatedList<BannerDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetBannersWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaginatedList<BannerDto>>> Handle(GetBannersWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();
        var query = _context.Banner.AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            var lowerSearch = request.SearchText.ToLower();
            query = query.Where(s => s.Title.ToLower().Contains(lowerSearch));
        }
        if (request.IsMobile)
        {
            var today = DateTime.UtcNow.Date;

            query = query
                .Where(x => x.IsActive)
                .Where(x =>
                    (!x.ShowFrom.HasValue || x.ShowFrom.Value.Date <= today) &&
                    (!x.ShowTo.HasValue || x.ShowTo.Value.Date >= today)
                );
        }
        var paginationResult = await query
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Created)
            .ProjectTo<BannerDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<BannerDto>>.Success(paginationResult);
    }
}