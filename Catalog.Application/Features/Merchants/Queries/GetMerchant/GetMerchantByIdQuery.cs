using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Merchants.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Merchants.Queries.GetMerchant;

public record GetMerchantByIdQuery: IRequest<ApiResponse<MerchantDto>>
{
	public int Id { get; init; }
}

public class GetMerchantByIdQueryHandler : IRequestHandler<GetMerchantByIdQuery, ApiResponse<MerchantDto>>
{
    private readonly ICatalogDbContext _contexxt;
    private readonly IMapper _mapper;

    public GetMerchantByIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _contexxt = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<MerchantDto>> Handle(GetMerchantByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _contexxt.Merchant
            .AsNoTracking()
            .Include(x => x.MerchantContractImages)
            .Include(x => x.Brands)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(Store), request.Id);
        }

        return ApiResponse<MerchantDto>.Success(_mapper.Map<MerchantDto>(entity));

    }
}
