using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Services.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Services.Queries.GetServices;

public record GetServiceByTechnicianIdsQuery : IRequest<ApiResponse<IEnumerable<ServiceDto>>>
{
    public string TechnicianIds { get; init; } = null!;
}
public class GetServiceByTechnicianIdsQueryHandler : IRequestHandler<GetServiceByTechnicianIdsQuery, ApiResponse<IEnumerable<ServiceDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetServiceByTechnicianIdsQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<ServiceDto>>> Handle(GetServiceByTechnicianIdsQuery request, CancellationToken cancellationToken)
    {
        var technicianIds = request.TechnicianIds.Split(",");
        var servicePackageIds = await _context.UserStore
        .AsNoTracking()
        .Where(x => x.UserId != null &&
                    technicianIds.Contains(x.UserId))
        .Join(
            _context.Store,
            us => us.StoreId,
            s => s.Id,
            (us, s) => s.ServicePackageId)
        .Where(x => x.HasValue)
        .Select(x => x!.Value)
        .Distinct()
        .ToListAsync(cancellationToken);

    var services = await _context.Service
        .AsNoTracking()
        .Where(x => x.ServicePackages
            .Any(sp => servicePackageIds.Contains(sp.Id)))
        .Distinct()
        .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<ServiceDto>>.Success(_mapper.Map<IEnumerable<ServiceDto>>(services));
    }
}