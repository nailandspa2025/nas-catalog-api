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
    public string? SearchText { get; init; }

}

public class GetServiceByStoreIdQueryHandler : IRequestHandler<GetServiceByStoreIdQuery, ApiResponse<IEnumerable<ServiceDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetServiceByStoreIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<IEnumerable<ServiceDto>>> Handle(GetServiceByStoreIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Service
            .Include(x => x.ServicePackages)
            .ThenInclude(x => x.Stores)
            .Where(service =>
                service.ServicePackages.Any(sp =>
                    sp.Stores.Any(store => store.Id == request.StoreId)));

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.Trim().ToLower();
            query = query.Where(service =>
                service.Name.ToLower().Contains(searchText) ||            
                service.Code.ToLower().Contains(searchText));             
        }

        var entities = await query.ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<ServiceDto>>.Success(_mapper.Map<IEnumerable<ServiceDto>>(entities));
    }
}