using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.BankAccounts.Commands.DeleteBankAccount;

public record DeleteBankAccountCommand(int Id) : IRequest<ApiResponse>;

public class DeleteBankAccountCommandHandler : IRequestHandler<DeleteBankAccountCommand, ApiResponse>
{
    private readonly ICatalogDbContext _context;

    public DeleteBankAccountCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }
    public async Task<ApiResponse> Handle(DeleteBankAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BankAccount
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(BankAccount), request.Id);
        }

        _context.BankAccount.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}