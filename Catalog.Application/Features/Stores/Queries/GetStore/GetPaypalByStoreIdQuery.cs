using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Stores.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Stores.Queries.GetStore;

public record GetPaypalByStoreIdQuery: IRequest<ApiResponse<PayPalConfigDto>>
{
    public long StoreId { get; set; }
}

public class GetPaypalByStoreIdQueryHandler : IRequestHandler<GetPaypalByStoreIdQuery, ApiResponse<PayPalConfigDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetPaypalByStoreIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PayPalConfigDto>> Handle(GetPaypalByStoreIdQuery request, CancellationToken cancellationToken)
    {
        var config = await _context.Store
            .AsNoTracking()
            .Where(x => x.Id == request.StoreId)
            .Select(x => x.PayPalConfig) 
            .FirstOrDefaultAsync(cancellationToken);

        if (config == null)
            throw new NotFoundException(nameof(Store), request.StoreId);

        return ApiResponse<PayPalConfigDto>.Success(_mapper.Map<PayPalConfigDto>(config));
    }
}
