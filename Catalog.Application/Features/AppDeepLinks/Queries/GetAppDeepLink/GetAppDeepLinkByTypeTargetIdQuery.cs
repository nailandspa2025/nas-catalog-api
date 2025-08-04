using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.AppDeepLinks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.AppDeepLinks.Queries.GetAppDeepLink;

public record GetAppDeepLinkByTypeTargetIdQuery : IRequest<ApiResponse<AppDeepLinkDto>>
{
    public string Type { get; init; } = null!;
    public string Id { get; init; } = null!;

}

public class GetAppDeepLinkByCodeTargetIdQueryHandler : IRequestHandler<GetAppDeepLinkByTypeTargetIdQuery, ApiResponse<AppDeepLinkDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetAppDeepLinkByCodeTargetIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AppDeepLinkDto>> Handle(GetAppDeepLinkByTypeTargetIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.AppDeepLink
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.Type == request.Type && x.TargetId == request.Id && !x.IsDeleted);

        if (entity == null)
        {
            return ApiResponse<AppDeepLinkDto>.Error("Link not found");
        }
        return ApiResponse< AppDeepLinkDto >.Success(_mapper.Map<AppDeepLinkDto>(entity));
    }
}
