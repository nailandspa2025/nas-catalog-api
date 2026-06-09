using BuildingBlocks.Core.Response;
using Catalog.Application.Features.Stores.Commands.CreateStore;
using Catalog.Application.Features.Stores.Commands.DeleteStore;
using Catalog.Application.Features.Stores.Commands.UpdateStore;
using Catalog.Application.Features.Stores.Models;
using Catalog.Application.Features.Stores.Queries.GetPaymentProvider;
using Catalog.Application.Features.Stores.Queries.GetStore;
using Catalog.Application.Features.Stores.Queries.GetStoreForMerchantsWithPagination;
using Catalog.Application.Features.Stores.Queries.GetStoreForMobileWithPagination;
using Catalog.Application.Features.Stores.Queries.GetStores;
using Catalog.Application.Features.Stores.Queries.GetStoresWithPagination;
using Catalog.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class StoresController : ApiControllerBase
    {
        [HttpGet("pagingation")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedList<StoreDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaginatedList<StoreDto>>>> GetWithPaginationAsync([FromQuery] GetStoresWithPaginationQuery query)
        {
            return await Mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpGet("mobile-pagingation")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedList<StoreDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaginatedList<StoreDto>>>> GetWithPaginationForMobileAsync([FromQuery] GetStoreForMobileWithPaginationQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("merchant-pagingation")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedList<StoreDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaginatedList<StoreDto>>>> GetStoreForMerchantsWithPaginationAsync([FromQuery] GetStoreForMerchantsWithPaginationQuery query)
        {
            return await Mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<StoreDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<StoreDto>>> GetByIdAsync(long id)
        {
            return await Mediator.Send(new GetStoreByIdQuery { Id = id });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<StoreDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<StoreDto>>> CreateAsync([FromForm] CreateStoreCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<StoreDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<StoreDto>>> UpdateAsync(long id, [FromForm] UpdateStoreCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            return await Mediator.Send(command);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> DeleteAsync(long id)
        {
            return await Mediator.Send(new DeleteStoreCommand(id));
        }

        [HttpPut("update-favorite/{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> UpdateFavoriteAsync(int id, [FromForm] UpdateStoreFavoriteCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            return await Mediator.Send(command);
        }

        [HttpGet("ids")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<StoreDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<StoreDto>>>> GetByIdsAsync(string ids)
        {
            return await Mediator.Send(new GetCategoryByIsdQuery { Ids = ids });
        }

        [AllowAnonymous]
        [HttpGet("paypal-config/{storeId}")]
        public async Task<ActionResult<ApiResponse<PayPalConfigDto>>> GetPaypalConfigAsync(long storeId)
        {
            return await Mediator.Send(new GetPaypalByStoreIdQuery { StoreId = storeId });
        }

        [AllowAnonymous]
        [HttpGet("payment-providers/{storeId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PaymentProviderDto>>>> GetPaymentProviders(long storeId)
        {
            return await Mediator.Send(
            new GetPaymentProviderByStoreIdQuery
            {
                StoreId = storeId
            });
        }
        [AllowAnonymous]
        [HttpGet("payment-provider")]
        public async Task<ActionResult<ApiResponse<PaymentProviderDto>>>
            GetPaymentProviderAsync(
                long storeId,
                PaymentMethod paymentMethod)
        {
            return await Mediator.Send(
                new GetPaymentProviderByMethodQuery
                {
                    StoreId = storeId,
                    PaymentMethod = paymentMethod
                });
        }
    }
}
