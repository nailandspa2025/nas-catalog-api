using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.ServicePackages.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.ServicePackages.Queries.GetServicePackages;

public record GetServicePackageByIdsQuery : IRequest<ApiResponse<IEnumerable<ServicePackageDto>>>
{
    public string Ids { get; init; } = null!;
}

public class GetServicePackageByIdsQueryHandler : IRequestHandler<GetServicePackageByIdsQuery, ApiResponse<IEnumerable<ServicePackageDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetServicePackageByIdsQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<ServicePackageDto>>> Handle(GetServicePackageByIdsQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids.Split(",");

        var entities = await _context.ServicePackage
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<ServicePackageDto>>.Success(_mapper.Map<IEnumerable<ServicePackageDto>>(entities));
    }
}