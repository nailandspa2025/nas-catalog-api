using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.StoreMerchants.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.StoreMerchants.Queries.GetStoreMerchantsWithPagination;

public record GetStoreMerchantsWithPaginationQuery: IRequest<ApiResponse<PaginatedList<StoreMerchantDto>>>
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? SearchText { get; set; }
}

public class GetStoreMerchantsWithPaginationQueryHandler : IRequestHandler<GetStoreMerchantsWithPaginationQuery, ApiResponse<PaginatedList<StoreMerchantDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public GetStoreMerchantsWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper, ICurrentUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<PaginatedList<StoreMerchantDto>>> Handle(GetStoreMerchantsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();
        var query = _context.Store
            .Where(s => !s.IsDeleted &&
                        _context.UserStore.Any(us => us.UserId == _currentUser.UserId && us.StoreId == s.Id))
            .AsNoTracking();

        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(s => paramSearchText.Contains(s.StoreName));
        }

        var paginationResult = await query
            
            .OrderBy(x => x.Created)
            .ProjectTo<StoreMerchantDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<StoreMerchantDto>>.Success(paginationResult);

    }
}