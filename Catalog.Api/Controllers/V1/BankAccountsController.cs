using BuildingBlocks.Core.Response;
using Catalog.Application.Features.BankAccounts.Commands.CreateBankAccount;
using Catalog.Application.Features.BankAccounts.Commands.DeleteBankAccount;
using Catalog.Application.Features.BankAccounts.Commands.UpdateBankAccount;
using Catalog.Application.Features.BankAccounts.Models;
using Catalog.Application.Features.BankAccounts.Queries.GetBankAccount;
using Catalog.Application.Features.BankAccounts.Queries.GetBankAccountByStore;
using Catalog.Application.Features.BankAccounts.Queries.GetBankAccounts;
using Catalog.Application.Features.BankAccounts.Queries.GetBankAccountsWithPagination;
using Catalog.Application.Features.Services.Models;
using Catalog.Application.Features.Services.Queries.GetServiceByStoreId;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]

public class BankAccountsController: ApiControllerBase
{
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<BankAccountDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<BankAccountDto>>>> GetBannersWithPaginationAsync([FromQuery] GetBankAccountsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BankAccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BankAccountDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetBankAccountByIdQuery { Id = id });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BankAccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BankAccountDto>>> CreateAsync([FromForm] CreateBankAccountCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BankAccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<BankAccountDto>>> UpdateAsync(long id, [FromForm] UpdateBankAccountCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(int id)
    {
        return await Mediator.Send(new DeleteBankAccountCommand(id));
    }

    [HttpGet("ids")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BankAccountDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BankAccountDto>>>> GetByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetBankAccountByIdsQuery { Ids = ids });
    }

    [HttpGet("store")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BankAccountDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BankAccountDto>>>> GetByStoreIdAsync([FromQuery] GetBankAccountByStoreIdQuery query)
    {
        return await Mediator.Send(query);
    }
}