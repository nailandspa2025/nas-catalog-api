using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.AppDeepLinks.Models;
using Catalog.Application.Features.BankAccounts.Models;
using Google.Rpc;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Google.Rpc.Help.Types;

namespace Catalog.Application.Features.AppDeepLinks.Queries.GetAppDeepLink;

public record GetAppDeepLinkByCodeQuery : IRequest<ApiResponse<AppDeepLinkDto>>
{
    public string Code { get; init; } = null!;
}

public class GetAppDeepLinkByCodeQueryHandler : IRequestHandler<GetAppDeepLinkByCodeQuery, ApiResponse<AppDeepLinkDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetAppDeepLinkByCodeQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<AppDeepLinkDto>> Handle(GetAppDeepLinkByCodeQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.AppDeepLink
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.Code == request.Code && !x.IsDeleted);

        if (entity == null)
        {
            return ApiResponse<AppDeepLinkDto>.Error("Link not found");
        }
        var dto = _mapper.Map<AppDeepLinkDto>(entity);
        return ApiResponse<AppDeepLinkDto>.Success(dto);
    }
}
