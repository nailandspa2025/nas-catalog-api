using BuildingBlocks.Core.Response;
using Catalog.Application.Features.Produts.Models;
using Catalog.Application.Features.Produts.Queries.GetProduct;
using Catalog.Application.Features.Produts.Queries.GetProducts;
using Catalog.Application.Features.Produts.Queries.GetProductsWithPagination;
using Catalog.Application.Features.Stores.Models;
using Catalog.Application.Features.Stores.Queries.GetStore;
using Catalog.Application.Features.Stores.Queries.GetStores;
using Catalog.Application.Features.Stores.Queries.GetStoresWithPagination;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]
public class DropdownListController : ApiControllerBase
{
    [HttpGet("stores")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<StoreDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<StoreDto>>>> GetStoresnAsync([FromQuery] GetStoresWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }
    [HttpGet("store-ids/{ids}")]
    [ProducesResponseType(typeof(ApiResponse<StoreDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StoreDto>>>> GetStoreByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetStoreByIdsQuery { Ids = ids });
    }

    [HttpGet("store/{id}")]
    [ProducesResponseType(typeof(ApiResponse<StoreDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StoreDto>>> GetStoreByIdAsync(long id)
    {
        return await Mediator.Send(new GetStoreByIdQuery { Id = id });
    }

    [HttpGet("products")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<ProductDto>>>> GetProductsAsync([FromQuery] GetProductsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("product-ids/{ids}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProductByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetProductByIdsQuery { Ids = ids });
    }

    [HttpGet("product/{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProductByIdAsync(long id)
    {
        return await Mediator.Send(new GetProductByIdQuery { Id = id });
    }
}
