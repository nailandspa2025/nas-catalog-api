using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Queries.GetPaymentProvider;
public record GetPaymentProviderByStoreIdQuery : IRequest<ApiResponse<IEnumerable<PaymentProviderDto>>>
{
    public long StoreId { get; init; }
}

public class GetPaymentProviderByStoreIdQueryHandler : IRequestHandler<GetPaymentProviderByStoreIdQuery, ApiResponse<IEnumerable<PaymentProviderDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetPaymentProviderByStoreIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<IEnumerable<PaymentProviderDto>>> Handle(GetPaymentProviderByStoreIdQuery request, CancellationToken cancellationToken)
    {
        var providers = _context.PaymentProvider
            .AsNoTracking()
            .Where(x => x.StoreId == request.StoreId);
            //.Include(x => x.Settings);
        return ApiResponse<IEnumerable<PaymentProviderDto>>.Success(_mapper.Map<IEnumerable<PaymentProviderDto>>(providers));
    }
}