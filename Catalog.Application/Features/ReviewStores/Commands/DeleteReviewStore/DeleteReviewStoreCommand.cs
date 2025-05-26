using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.ReviewStores.Commands.DeleteReviewStore;

public record DeleteReviewStoreCommand (int Id): IRequest<ApiResponse>;


public class DeleteReviewStoreCommandHandler : IRequestHandler<DeleteReviewStoreCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public DeleteReviewStoreCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteReviewStoreCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ReviewStore
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(ReviewStore), request.Id);
        }

        _context.ReviewStore.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}