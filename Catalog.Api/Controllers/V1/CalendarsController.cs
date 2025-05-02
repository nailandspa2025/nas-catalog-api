using BuildingBlocks.Core.Response;
using Catalog.Application.Features.Calendars.Commands.CreateCalendar;
using Catalog.Application.Features.Calendars.Commands.DeleteCalendar;
using Catalog.Application.Features.Calendars.Commands.UpdateCaledar;
using Catalog.Application.Features.Calendars.Models;
using Catalog.Application.Features.Calendars.Queries.GetCalendars;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]
public class CalendarsController: ApiControllerBase
{
    [HttpGet("calendars")]
    [ProducesResponseType(typeof(ApiResponse<List<CalendarDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<CalendarDto>>>> GetCalendarsAsync([FromQuery] GetCalendarsQuery query)
    {
        return await Mediator.Send(query);
    }

    //[HttpGet("{id}")]
    //[ProducesResponseType(typeof(ApiResponse<CalendarTypeDto>), StatusCodes.Status200OK)]
    //public async Task<ActionResult<ApiResponse<CalendarDto>>> GetByIdAsync(int id)
    //{
    //    return await Mediator.Send(new GetCalendarByIdQuery { Id = id });
    //}

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CalendarDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CalendarDto>>> CreateAsync([FromForm] CreateCalendarCommand commnd)
    {
        return await Mediator.Send(commnd);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CalendarDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CalendarDto>>> UpdateAsync(int id, [FromForm] UpdateCalendarCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(int id, [FromQuery] DeleteCalendarCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        return await Mediator.Send(command);
    }
}

