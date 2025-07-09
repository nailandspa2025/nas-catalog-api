using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Services.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Services.Queries.GetServices;

public record GetServiceByIdsQuery : IRequest<ApiResponse<IEnumerable<ServiceDto>>>
{
    public string Ids { get; init; } = null!;
}
public class GetServiceByIdsQueryHandler : IRequestHandler<GetServiceByIdsQuery, ApiResponse<IEnumerable<ServiceDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetServiceByIdsQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<ServiceDto>>> Handle(GetServiceByIdsQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids.Split(",");

        var rewards = await _context.Service
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<ServiceDto>>.Success(_mapper.Map<IEnumerable<ServiceDto>>(rewards));
    }
}