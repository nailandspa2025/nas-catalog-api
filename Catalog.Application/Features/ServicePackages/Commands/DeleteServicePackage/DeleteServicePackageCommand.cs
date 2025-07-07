using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.ServicePackages.Commands.DeleteServicePackage;

public record DeleteServicePackageCommand(int Id) : IRequest<ApiResponse>;

public class DeleteServicePackageCommandHandler : IRequestHandler<DeleteServicePackageCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public DeleteServicePackageCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteServicePackageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ServicePackage
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(ServicePackage), request.Id);
        }

        _context.ServicePackage.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}