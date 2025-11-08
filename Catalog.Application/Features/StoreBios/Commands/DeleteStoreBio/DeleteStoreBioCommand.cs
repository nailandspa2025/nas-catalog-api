using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.StoreBios.Commands.DeleteStoreBio;

public record DeleteStoreBioCommand(int Id) : IRequest<ApiResponse>;

public class DeleteStoreBioCommandHandler : IRequestHandler<DeleteStoreBioCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public DeleteStoreBioCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteStoreBioCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.StoreBio
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Product), request.Id);
        }

        _context.StoreBio.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}