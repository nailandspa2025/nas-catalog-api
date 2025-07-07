using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.Services.Commands.DeleteService;

public record DeleteServiceCommand(int Id) : IRequest<ApiResponse>;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public DeleteServiceCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Service
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Service), request.Id);
        }

        _context.Service.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}