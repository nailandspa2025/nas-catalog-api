using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Merchants.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Merchants.Queries.GetMerchants;

public record GetMerchantByIdsQuery: IRequest<ApiResponse<IEnumerable<MerchantDto>>>
{
	public string Ids { get; init; } = null!;
}

public class GetMerchantByIdsQueryHandler : IRequestHandler<GetMerchantByIdsQuery, ApiResponse<IEnumerable<MerchantDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetMerchantByIdsQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<MerchantDto>>> Handle(GetMerchantByIdsQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids.Split(",");
        var stores = await _context.Merchant
            .Include(x => x.Brands)
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<MerchantDto>>.Success(_mapper.Map<IEnumerable<MerchantDto>>(stores));
    }
}
