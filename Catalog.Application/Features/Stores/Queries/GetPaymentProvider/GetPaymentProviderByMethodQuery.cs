using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Queries.GetPaymentProvider;
public record GetPaymentProviderByMethodQuery : IRequest<ApiResponse<PaymentProviderDto>>
{
    public long StoreId { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
}

public class GetPaymentProviderByMethodQueryHandler : IRequestHandler<GetPaymentProviderByMethodQuery, ApiResponse<PaymentProviderDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetPaymentProviderByMethodQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaymentProviderDto>> Handle(GetPaymentProviderByMethodQuery request, CancellationToken cancellationToken)
    {
        var provider = await _context.PaymentProvider
            .AsNoTracking()
            .Include(x => x.Settings)
            .Where(x =>
                x.StoreId == request.StoreId &&
                x.PaymentMethod == request.PaymentMethod &&
                x.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (provider == null)
        {
            return ApiResponse<PaymentProviderDto>.Error(
                "Payment provider not found.");
        }

        return ApiResponse<PaymentProviderDto>.Success(_mapper.Map<PaymentProviderDto>(provider));
    }
}