using BuildingBlocks.Core.Response;
using Catalog.Application.Features.CalendarTypes.Commands.CreateCalendarType;
using Catalog.Application.Features.CalendarTypes.Commands.DeleteCalendarType;
using Catalog.Application.Features.CalendarTypes.Commands.UpdateCalendarType;
using Catalog.Application.Features.CalendarTypes.Models;
using Catalog.Application.Features.CalendarTypes.Queries.GetCalendarType;
using Catalog.Application.Features.CalendarTypes.Queries.GetCalendarTypes;
using Catalog.Application.Features.CalendarTypes.Queries.GetCalendarTypesWithPagination;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;


[ApiVersion("1.0")]
public class CalendarTypesController: ApiControllerBase
{
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<CalendarTypeDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<CalendarTypeDto>>>> GetWithPaginationAsync([FromQuery] GetCalendarTypesWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CalendarTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CalendarTypeDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetCalendarTypeByIdQuery { Id = id });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CalendarTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CalendarTypeDto>>> CreateAsync([FromForm] CreateCalendarTypeCommand commnd)
    {
        return await Mediator.Send(commnd);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CalendarTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CalendarTypeDto>>> UpdateAsync(int id, [FromForm] UpdateCalendarTypeCommand command)
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
        return await Mediator.Send(new DeleteCalendarTypeCommand(id));
    }

    [HttpGet("{ids}")]
    [ProducesResponseType(typeof(ApiResponse<CalendarTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CalendarTypeDto>>>> GetByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetCalendarTypeByIdsQuery { Ids = ids });
    }
}

