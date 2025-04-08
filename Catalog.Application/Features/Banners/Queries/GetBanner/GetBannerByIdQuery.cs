using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Banners.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Banners.Queries.GetBanner;

public record GetBannerByIdQuery: IRequest<ApiResponse<BannerDto>>
{
    public int Id { get; set; }
}

public class GetBannerByIdQueryHandler : IRequestHandler<GetBannerByIdQuery, ApiResponse<BannerDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetBannerByIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BannerDto>> Handle(GetBannerByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Banner
            .AsNoTracking()
            .Include(x => x.ImageGallerys)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Banner), request.Id);
        }

        return ApiResponse<BannerDto>.Success(_mapper.Map<BannerDto>(entity));
    }
}