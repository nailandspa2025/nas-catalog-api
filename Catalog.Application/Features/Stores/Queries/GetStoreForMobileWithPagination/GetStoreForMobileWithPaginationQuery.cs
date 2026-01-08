using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Queries.GetStoreForMobileWithPagination;

public record GetStoreForMobileWithPaginationQuery : IRequest<ApiResponse<PaginatedList<StoreDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; init; }
    public int? Rating { get; init; }
    public double? Lat { get; init; }
    public double? Long { get; init; }
    public bool? IsFavorite { get; init; }

}

public class GetStoreForMobileWithPaginationQueryHandler : IRequestHandler<GetStoreForMobileWithPaginationQuery, ApiResponse<PaginatedList<StoreDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetStoreForMobileWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaginatedList<StoreDto>>> Handle(GetStoreForMobileWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();
        var query = _context.Store
            .Where(s => !s.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(paramSearchText))
        {
            query = query.Where(s => s.StoreName.ToUpper().Contains(paramSearchText)
            || s.AddressStore.ToUpper().Contains(paramSearchText));
        }
        if (request.Rating.HasValue)
        {
            query = query.Where(s => s.RatingStar == request.Rating.Value);
        }
        if (request.IsFavorite.HasValue)
        {
            query = query.Where(s => s.IsFavorite == request.IsFavorite.Value);
        }
        if (request.Lat.HasValue && request.Long.HasValue)
        {
            var lat = request.Lat.Value;
            var lon = request.Long.Value;
            query = query.OrderBy(s =>
                6371 * Math.Acos(
                    Math.Cos(lat * Math.PI / 180) *
                    Math.Cos(s.Lat * Math.PI / 180) *
                    Math.Cos((s.Lng - lon) * Math.PI / 180) +
                    Math.Sin(lat * Math.PI / 180) *
                    Math.Sin(s.Lat * Math.PI / 180)
                )
            );
        }
        else
        {
            query = query.OrderBy(x => x.Order).ThenByDescending(x => x.Created);
        }
        var paginationResult = await query
            .ProjectTo<StoreDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<StoreDto>>.Success(paginationResult);
    }
    private static double ToRadians(double angle)
    => Math.PI * angle / 180.0;
}