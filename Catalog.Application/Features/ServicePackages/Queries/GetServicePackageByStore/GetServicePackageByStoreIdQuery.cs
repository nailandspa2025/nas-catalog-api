using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ServicePackages.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.ServicePackages.Queries.GetServicePackageByStore;

public record GetServicePackageByStoreIdQuery: IRequest<ApiResponse<IEnumerable<ServicePackageDto>>>
{
    public long StoreId { get; init; }
}

public class GetServicePackageByStoreIdQueryHander : IRequestHandler<GetServicePackageByStoreIdQuery, ApiResponse<IEnumerable<ServicePackageDto>>>
{
    private readonly ICatalogDbContext _contexxt;
    private readonly IMapper _mapper;

    public GetServicePackageByStoreIdQueryHander(ICatalogDbContext context, IMapper mapper)
    {
        _contexxt = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<IEnumerable<ServicePackageDto>>> Handle(GetServicePackageByStoreIdQuery request, CancellationToken cancellationToken)
    {
        var entities = await _contexxt.ServicePackage
            .Where(sp => sp.Stores.Any(s => s.Id == request.StoreId))
            .ToListAsync(cancellationToken);
        return ApiResponse<IEnumerable<ServicePackageDto>>.Success(_mapper.Map<IEnumerable<ServicePackageDto>>(entities));
    }
}