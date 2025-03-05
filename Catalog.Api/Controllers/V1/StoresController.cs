using BuildingBlocks.Core.Response;
using Catalog.Application.Features.Produts.Commands.CreateProduct;
using Catalog.Application.Features.Produts.Commands.UpdateProduct;
using Catalog.Application.Features.Produts.Models;
using Catalog.Application.Features.Produts.Queries.GetProduct;
using Catalog.Application.Features.Produts.Queries.GetProductsWithPagination;
using Catalog.Application.Features.Stores.Commands.CreateStore;
using Catalog.Application.Features.Stores.Commands.DeleteStore;
using Catalog.Application.Features.Stores.Commands.UpdateStore;
using Catalog.Application.Features.Stores.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class StoresController : ApiControllerBase
    {
        [HttpGet("pagingation")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedList<ProductDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaginatedList<ProductDto>>>> GetWithPaginationAsync([FromQuery] GetProductsWithPaginationQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetByIdAsync(long id)
        {
            return await Mediator.Send(new GetProductByIdQuery { Id = id });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<StoreDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<StoreDto>>> CreateAsync(CreateStoreCommand commnd)
        {
            return await Mediator.Send(commnd);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<StoreDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<StoreDto>>> UpdateAsync(long id, UpdateStoreCommand command)
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
    }
}
