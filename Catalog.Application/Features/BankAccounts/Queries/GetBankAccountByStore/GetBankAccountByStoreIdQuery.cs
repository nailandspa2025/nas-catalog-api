using AutoMapper;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.BankAccounts.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.BankAccounts.Queries.GetBankAccountByStore;

public record GetBankAccountByStoreIdQuery: IRequest<ApiResponse<IEnumerable<BankAccountDto>>>
{
    public long StoreId { get; set; }
    public string? SearchText { get; init; }
}

public class GetBankAccountByStoreIdQueryHandler : IRequestHandler<GetBankAccountByStoreIdQuery, ApiResponse<IEnumerable<BankAccountDto>>>
{

    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetBankAccountByStoreIdQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<IEnumerable<BankAccountDto>>> Handle(GetBankAccountByStoreIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.BankAccount
            .Where(x => x.Stores.Any(s => s.Id == request.StoreId));

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.Trim().ToLower();
            query = query.Where(s =>
                s.BankName.ToLower().Contains(searchText)
                || s.AccountNumber.ToLower().Contains(searchText)
                || s.AccountName.ToLower().Contains(searchText));
        }
        var entities = await query.ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<BankAccountDto>>.Success(_mapper.Map<IEnumerable<BankAccountDto>>(entities));
    }
}
