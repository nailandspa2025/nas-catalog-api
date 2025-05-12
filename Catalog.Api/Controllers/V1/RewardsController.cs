using BuildingBlocks.Core.Response;
using Catalog.Application.Features.Rewards.Commands.CreateReward;
using Catalog.Application.Features.Rewards.Commands.DeleteReward;
using Catalog.Application.Features.Rewards.Commands.UpdateReward;
using Catalog.Application.Features.Rewards.Models;
using Catalog.Application.Features.Rewards.Queries.GetReward;
using Catalog.Application.Features.Rewards.Queries.GetRewardsWithPagination;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;


[ApiVersion("1.0")]
public class RewardsController: ApiControllerBase
{
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<RewardDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<RewardDto>>>> GetRewardsWithPaginationAsync([FromQuery] GetRewardsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<RewardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<RewardDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetRewardByIdQuery { Id = id });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RewardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<RewardDto>>> CreateAsync([FromForm] CreateRewardCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<RewardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<RewardDto>>> UpdateAsync(int id, [FromForm] UpdateRewardCommand command)
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
        return await Mediator.Send(new DeleteRewardCommand(id));
    }
}

