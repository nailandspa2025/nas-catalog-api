using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Services.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Services.Queries.GetServiceByStoreId;

public record GetServiceByStoreIdQuery: IRequest<ApiResponse<IEnumerable<ServiceDto>>>
{
    public long StoreId { get; set; }
}


public class GetServiceByStoreIdQueryHandler : IRequestHandler<GetServiceByStoreIdQuery, ApiResponse<IEnumerable<ServiceDto>>>
{
    private readonly ICatalogDbContext _contexxt;
    private readonly IMapper _mapper;

    public GetServiceByStoreIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _contexxt = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<IEnumerable<ServiceDto>>> Handle(GetServiceByStoreIdQuery request, CancellationToken cancellationToken)
    {
        var entities = await _contexxt.Service
            .Include(x => x.ServicePackages)
            .ThenInclude(x => x.Stores)
            .Where(service =>
            service.ServicePackages.Any(sp =>
                sp.Stores.Any(store => store.Id == request.StoreId)))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<ServiceDto>>.Success(_mapper.Map<IEnumerable<ServiceDto>>(entities));
    }
}