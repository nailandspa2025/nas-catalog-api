using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Catalog.Application.Features.Merchants.Commads.CreateMerchant;
using Catalog.Application.Features.Merchants.Commads.DeleteMerchant;
using Catalog.Application.Features.Merchants.Commads.UpdateMerchant;
using Catalog.Application.Features.Merchants.Models;
using Catalog.Application.Features.Merchants.Queries.GetMerchant;
using Catalog.Application.Features.Merchants.Queries.GetMerchants;
using Catalog.Application.Features.Merchants.Queries.GetMerchantsWithPagination;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]
public class MerchantsController: ApiControllerBase
{
    [AccessGroup("merchant.view")]
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<MerchantDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<MerchantDto>>>> GetWithPaginationAsync([FromQuery] GetMerchantsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [AccessGroup("merchant.view")]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<MerchantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<MerchantDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetMerchantByIdQuery { Id = id });
    }

    [AccessGroup("merchant.view")]
    [HttpGet("ids")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MerchantDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MerchantDto>>>> GetByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetMerchantByIdsQuery { Ids = ids });
    }

    [AccessGroup("merchant.create")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MerchantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<MerchantDto>>> CreateAsync([FromForm] CreateMerchantCommand command)
    {
        return await Mediator.Send(command);
    }

    [AccessGroup("merchant.update")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<MerchantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<MerchantDto>>> UpdateAsync(int id, [FromForm] UpdateMerchantCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        return await Mediator.Send(command);
    }

    [AccessGroup("merchant.delete")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync (int id)
    {
        return await Mediator.Send(new DeleteMerchantCommand(id));
    }
}

