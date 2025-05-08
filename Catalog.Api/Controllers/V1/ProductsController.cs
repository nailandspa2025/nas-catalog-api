using BuildingBlocks.Core.Response;
using Catalog.Application.Features.Produts.Commands.CreateProduct;
using Catalog.Application.Features.Produts.Commands.DeleteProduct;
using Catalog.Application.Features.Produts.Commands.UpdateProduct;
using Catalog.Application.Features.Produts.Models;
using Catalog.Application.Features.Produts.Queries.GetProduct;
using Catalog.Application.Features.Produts.Queries.GetProductsWithPagination;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class ProductsController : ApiControllerBase
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
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> CreateAsync([FromForm] CreateProductCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateAsync(long id, [FromForm] UpdateProductCommand command)
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
            return await Mediator.Send(new DeleteProductCommand(id));
        }
    }
}
