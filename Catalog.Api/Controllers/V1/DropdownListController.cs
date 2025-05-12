using BuildingBlocks.Core.Response;
using Catalog.Application.Features.CalendarTypes.Models;
using Catalog.Application.Features.CalendarTypes.Queries.GetCalendarType;
using Catalog.Application.Features.CalendarTypes.Queries.GetCalendarTypes;
using Catalog.Application.Features.CalendarTypes.Queries.GetCalendarTypesWithPagination;
using Catalog.Application.Features.Merchants.Models;
using Catalog.Application.Features.Merchants.Queries.GetMerchant;
using Catalog.Application.Features.Merchants.Queries.GetMerchants;
using Catalog.Application.Features.Merchants.Queries.GetMerchantsWithPagination;
using Catalog.Application.Features.Produts.Models;
using Catalog.Application.Features.Produts.Queries.GetProduct;
using Catalog.Application.Features.Produts.Queries.GetProducts;
using Catalog.Application.Features.Produts.Queries.GetProductsWithPagination;
using Catalog.Application.Features.Rewards.Models;
using Catalog.Application.Features.Rewards.Queries.GetReward;
using Catalog.Application.Features.Rewards.Queries.GetRewards;
using Catalog.Application.Features.Rewards.Queries.GetRewardsWithPagination;
using Catalog.Application.Features.Stores.Models;
using Catalog.Application.Features.Stores.Queries.GetStore;
using Catalog.Application.Features.Stores.Queries.GetStoreForMerchantsWithPagination;
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
    [HttpGet("merchant-stores")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<StoreDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<StoreDto>>>> GetStoreForMerchantnAsync([FromQuery] GetStoreForMerchantsWithPaginationQuery query)
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


    [HttpGet("calendar-types")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<CalendarTypeDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<CalendarTypeDto>>>> GetCalendarTypesAsync([FromQuery] GetCalendarTypesWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("calendar-type/ids/{ids}")]
    [ProducesResponseType(typeof(ApiResponse<CalendarTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CalendarTypeDto>>>> GetCalendarTypeByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetCalendarTypeByIdsQuery { Ids = ids });
    }

    [HttpGet("calendar-type/{id}")]
    [ProducesResponseType(typeof(ApiResponse<CalendarTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CalendarTypeDto>>> GetCalendarTypeByIdAsync(int id)
    {
        return await Mediator.Send(new GetCalendarTypeByIdQuery { Id = id });
    }

    [HttpGet("merchants")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<MerchantDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<MerchantDto>>>> GetMerchantsAsync([FromQuery] GetMerchantsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("merchant-ids/{ids}")]
    [ProducesResponseType(typeof(ApiResponse<MerchantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MerchantDto>>>> GetMerchantByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetMerchantByIdsQuery { Ids = ids });
    }

    [HttpGet("merchant/{id}")]
    [ProducesResponseType(typeof(ApiResponse<MerchantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<MerchantDto>>> GetMerchantByIdAsync(int id)
    {
        return await Mediator.Send(new GetMerchantByIdQuery { Id = id });
    }

    [HttpGet("rewards")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<RewardDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<RewardDto>>>> GetRewardsAsync([FromQuery] GetRewardsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("reward-ids/{ids}")]
    [ProducesResponseType(typeof(ApiResponse<RewardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<RewardDto>>>> GetRewardByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetRewardByIdsQuery { Ids = ids });
    }

    [HttpGet("rewards/{id}")]
    [ProducesResponseType(typeof(ApiResponse<RewardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<RewardDto>>> GetRewardByIdAsync(int id)
    {
        return await Mediator.Send(new GetRewardByIdQuery { Id = id });
    }
}
