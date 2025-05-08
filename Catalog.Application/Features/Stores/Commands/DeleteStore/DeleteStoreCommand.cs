using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.Stores.Commands.DeleteStore;

public record DeleteStoreCommand(long Id): IRequest<ApiResponse>;

public class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public DeleteStoreCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteStoreCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Store
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Store), request.Id);
        }

        _context.Store.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}

