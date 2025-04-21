using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.Banners.Commands.DeleteBanner;

public record DeleteBannerCommand(int Id): IRequest<ApiResponse>;

public class DeleteBannerCommandHandler : IRequestHandler<DeleteBannerCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public DeleteBannerCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteBannerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Banner
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Product), request.Id);
        }

        _context.Banner.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}