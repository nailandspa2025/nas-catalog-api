using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.BankAccounts.Models;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.BankAccounts.Queries.GetBankAccount;

public record GetBankAccountByIdQuery: IRequest<ApiResponse<BankAccountDto>>
{
    public int Id { get; set; }
}

public class GetBankAccountByIdQueryHandler : IRequestHandler<GetBankAccountByIdQuery, ApiResponse<BankAccountDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetBankAccountByIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<BankAccountDto>> Handle(GetBankAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.BankAccount
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(BankAccount), request.Id);
        }
        return ApiResponse<BankAccountDto>.Success(_mapper.Map<BankAccountDto>(entity));
    }
}
