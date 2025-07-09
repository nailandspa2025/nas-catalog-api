using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.BankAccounts.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.BankAccounts.Queries.GetBankAccountsWithPagination;

public record GetBankAccountsWithPaginationQuery: IRequest<ApiResponse<PaginatedList<BankAccountDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; init; }
}

public class GetBankAccountsWithPaginationQueryHandler : IRequestHandler<GetBankAccountsWithPaginationQuery, ApiResponse<PaginatedList<BankAccountDto>>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;

    public GetBankAccountsWithPaginationQueryHandler(ICatalogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaginatedList<BankAccountDto>>> Handle(GetBankAccountsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = (request.SearchText ?? string.Empty).ToUpper();
        var query = _context.BankAccount.Where(x => !x.IsDeleted).AsNoTracking();

        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(
                s => s.BankName.ToUpper().Contains(paramSearchText) 
                || s.AccountNumber.ToUpper().Contains(paramSearchText)
                || s.BranchName.ToUpper().Contains(paramSearchText)
                || s.AccountName.ToUpper().Contains(paramSearchText)
            );
        }

        var paginationResult = await query
            .OrderBy(x => x.Created)
            .ProjectTo<BankAccountDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<BankAccountDto>>.Success(paginationResult);
    }
}