using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.BankAccounts.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.BankAccounts.Queries.GetBankAccounts;

public record GetBankAccountByIdsQuery: IRequest<ApiResponse<IEnumerable<BankAccountDto>>>
{
    public string Ids { get; init; } = null!;
}

public class GetBankAccountByIdsQueryHandler : IRequestHandler<GetBankAccountByIdsQuery, ApiResponse<IEnumerable<BankAccountDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetBankAccountByIdsQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<BankAccountDto>>> Handle(GetBankAccountByIdsQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids.Split(",");
        var entities = await _context.BankAccount
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<BankAccountDto>>.Success(_mapper.Map<IEnumerable<BankAccountDto>>(entities));
    }
}
