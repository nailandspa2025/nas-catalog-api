using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Catalog.Application.Features.StoreMerchants.Models;
using Catalog.Application.Features.StoreMerchants.Queries.GetStoreMerchantsWithPagination;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]
public class StoreMerchantsController: ApiControllerBase
{
    [AccessGroup("store.view")]
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<StoreMerchantDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<StoreMerchantDto>>>> GetWithPaginationAsync([FromQuery] GetStoreMerchantsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }
}

