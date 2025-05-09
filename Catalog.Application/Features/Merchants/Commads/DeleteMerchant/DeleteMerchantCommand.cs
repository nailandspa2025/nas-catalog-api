using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;
namespace Catalog.Application.Features.Merchants.Commads.DeleteMerchant;

public record DeleteMerchantCommand(int Id) : IRequest<ApiResponse>;

public class DeleteMerchantCommandHandler : IRequestHandler<DeleteMerchantCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public DeleteMerchantCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteMerchantCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Merchant
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Merchant), request.Id);
        }
        _context.Merchant.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}


