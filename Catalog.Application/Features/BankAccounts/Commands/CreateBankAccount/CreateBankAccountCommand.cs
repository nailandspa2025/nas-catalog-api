using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.BankAccounts.Models;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.BankAccounts.Commands.CreateBankAccount;

public record CreateBankAccountCommand: IRequest<ApiResponse<BankAccountDto>>
{
    public string AccountName { get; init; } = null!;
    public string AccountNumber { get; init; } = null!;
    public string BankName { get; init; } = null!;
    public string BranchName { get; init; } = null!;
    public string? SwiftCode { get; init; }
    public string? CurrencyCode { get; init; }
}

public class CreateBankAccountCommandHandler : IRequestHandler<CreateBankAccountCommand, ApiResponse<BankAccountDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public CreateBankAccountCommandHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<BankAccountDto>> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = new BankAccount
        {
            BankName = request.BankName,
            BranchName = request.BranchName,
            SwiftCode = request.SwiftCode,
            CurrencyCode = request.CurrencyCode,
            AccountName = request.AccountName,
            AccountNumber = request.AccountNumber,
        };
        _context.BankAccount.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<BankAccountDto>.Success(_mapper.Map<BankAccountDto>(entity));
    }
}
