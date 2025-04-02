using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.Stores.Commands.UpdateStore;

public record UpdateStoreFavoriteCommand: IRequest<ApiResponse>
{
    public long Id { get; init; }

    public bool IsFavorite { get; init; }
}

public class UpdateStoreFavoriteCommandHandler : IRequestHandler<UpdateStoreFavoriteCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public UpdateStoreFavoriteCommandHandler(ICatalogDbContext context, IMapper mapper, IStorageService storageService)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(UpdateStoreFavoriteCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Store
            .FindAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Store), request.Id);
        }
        entity.IsFavorite = request.IsFavorite;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}
