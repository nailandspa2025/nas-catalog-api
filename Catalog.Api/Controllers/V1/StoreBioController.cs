using BuildingBlocks.Core.Response;
using Catalog.Application.Features.StoreBios.Commands.CreateStoreBio;
using Catalog.Application.Features.StoreBios.Commands.DeleteStoreBio;
using Catalog.Application.Features.StoreBios.Commands.UpdateStoreBio;
using Catalog.Application.Features.StoreBios.Models;
using Catalog.Application.Features.StoreBios.Queries.GetStoreBio;
using Catalog.Application.Features.StoreBios.Queries.GetStoreBios;
using Catalog.Application.Features.StoreBios.Queries.GetStoreBioWithPagination;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]
public class StoreBioController: ApiControllerBase
{
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<StoreBioDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<StoreBioDto>>>> GetWithPaginationAsync([FromQuery] GetStoreBioWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<StoreBioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StoreBioDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetStoreBioByIdQuery { Id = id });
    }

    [HttpGet("ids")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StoreBioDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StoreBioDto>>>> GetByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetStoreBioByIdsQuery { Ids = ids });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StoreBioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StoreBioDto>>> CreateAsync([FromForm] CreateStoreBioCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<StoreBioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<StoreBioDto>>> UpdateAsync(long id, [FromForm] UpdateStoreBioCommand command)
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
        return await Mediator.Send(new DeleteStoreBioCommand(id));
    }
}

