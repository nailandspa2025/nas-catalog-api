
using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.BankAccounts.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.BankAccounts.Commands.UpdateBankAccount;

public record UpdateBankAccountCommand: IRequest<ApiResponse<BankAccountDto>>
{
    public int Id { get; set; }
    public string AccountName { get; init; } = null!;
    public string AccountNumber { get; init; } = null!;
    public string BankName { get; init; } = null!;
    public string BranchName { get; init; } = null!;
    public string? SwiftCode { get; init; }
    public string? CurrencyCode { get; init; }
}

public class UpdateBankAccountCommandHandler : IRequestHandler<UpdateBankAccountCommand, ApiResponse<BankAccountDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public UpdateBankAccountCommandHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<BankAccountDto>> Handle(UpdateBankAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BankAccount
           .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(BankAccount), request.Id);
        }

        entity.AccountName = request.AccountName;
        entity.AccountNumber = request.AccountNumber;
        entity.BankName = request.BankName;
        entity.BranchName = request.BranchName;
        entity.SwiftCode = request.SwiftCode;
        entity.CurrencyCode = request.CurrencyCode;

        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<BankAccountDto>.Success(_mapper.Map<BankAccountDto>(entity));
    }
}
