using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Merchants.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Merchants.Queries.GetMerchantsWithPagination;

public record GetMerchantsWithPaginationQuery: IRequest<ApiResponse<PaginatedList<MerchantDto>>>
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? SearchText { get; set; }

    public bool? IsActive { get; init; }

    public DateTime?  ContractDate { get; init; }

    public DateTime? DeploymentDate { get; init; }

    public List<int> ServiePakages { get; init; } = new List<int>();

}

public class GetMerchantsWithPaginationQueryHandler : IRequestHandler<GetMerchantsWithPaginationQuery, ApiResponse<PaginatedList<MerchantDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetMerchantsWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaginatedList<MerchantDto>>> Handle(GetMerchantsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();

        var query = _context.Merchant.Where(x => !x.IsDeleted).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(s => paramSearchText.Contains(s.Name)
            || paramSearchText.Contains(s.Email)
            || paramSearchText.Contains(s.PhoneNumber)
            || paramSearchText.Contains(s.TaxCode)
            || paramSearchText.Contains(s.ContractNumber)
            );
        }
        if(request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }
        if (request.ContractDate.HasValue)
        {
            query = query.Where(m => m.ContractDate.Value.Date == request.ContractDate.Value.Date);
        }

        if (request.DeploymentDate.HasValue)
        {
            query = query.Where(m => m.DeploymentDate.Value.Date == request.DeploymentDate.Value.Date);
        }
        if (request.ServiePakages != null && request.ServiePakages.Any())
        {
            query = query.Where(m => request.ServiePakages.Contains(m.ServicePackageId.Value));
        }
        var paginationResult = await query
            .OrderBy(x => x.Created)
            .ProjectTo<MerchantDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<MerchantDto>>.Success(paginationResult);
    }
}