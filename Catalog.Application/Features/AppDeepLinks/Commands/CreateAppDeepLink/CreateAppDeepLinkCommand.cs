using AutoMapper;
using BuildingBlocks.Common.Helpers;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.AppDeepLinks.Models;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.AppDeepLinks.Commands.CreateAppDeepLink;

public class CreateAppDeepLinkCommand: IRequest<ApiResponse<AppDeepLinkDto>>
{
    public string Type { get; init; } = null!;
    public string TargetId { get; init; } = null!;
    public string AndroidLink { get; init; } 
    public string IOSLink { get; init; }
    public string WebFallback { get; init; }
}

public class CreateAppDeepLinkCommandHandler : IRequestHandler<CreateAppDeepLinkCommand, ApiResponse<AppDeepLinkDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    public CreateAppDeepLinkCommandHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public  async Task<ApiResponse<AppDeepLinkDto>> Handle(CreateAppDeepLinkCommand request, CancellationToken cancellationToken)
    {
        var code = StringGenerateRandom.Generate();
        var entity = new AppDeepLink
        {
            Type = request.Type,
            TargetId = request.TargetId,
            Code = code,
            AndroidLink = request.AndroidLink,
            IOSLink =  request.IOSLink,
            WebFallback = request.WebFallback,
        };
        _context.AppDeepLink.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<AppDeepLinkDto>.Success(_mapper.Map<AppDeepLinkDto>(entity));
    }
}
