using BuildingBlocks.Core.Response;
using Catalog.Application.Features.ReviewStores.Commands.CreateReviewStore;
using Catalog.Application.Features.ReviewStores.Commands.DeleteReviewStore;
using Catalog.Application.Features.ReviewStores.Commands.UpdateReviewStore;
using Catalog.Application.Features.ReviewStores.Models;
using Catalog.Application.Features.ReviewStores.Queries.GetReviewStore;
using Catalog.Application.Features.ReviewStores.Queries.GetReviewStores;
using Catalog.Application.Features.ReviewStores.Queries.GetReviewStoresWithPagination;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]
public class ReviewStoreController: ApiControllerBase
{
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ReviewStoreDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<ReviewStoreDto>>>> GetWithPaginationAsync([FromQuery] GetReviewStoresWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("mobile-pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ReviewStoreDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<ReviewStoreDto>>>> GetWithPaginationForMobileAsync([FromQuery] GetReviewStoresWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ReviewStoreDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ReviewStoreDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetReviewStoreByIdQuery { Id = id });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReviewStoreDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ReviewStoreDto>>> CreateAsync([FromForm] CreateReviewStoreCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ReviewStoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ReviewStoreDto>>> UpdateAsync(long id, [FromForm] UpdateReviewStoreCommand command)
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
        return await Mediator.Send(new DeleteReviewStoreCommand(id));
    }

    [HttpGet("technician")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ReviewTechnicianDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<ReviewTechnicianDto>>>> GetTechnicianIdWithPaginationAsync([FromQuery] GetReviewStoreByTechnicianIdsQuery query)
    {
        return await Mediator.Send(query);
    }

}

