using BuildingBlocks.Core.Response;
using Catalog.Application.Features.UserStores.Models;
using Catalog.Application.Features.UserStores.Queries.GetUserStore;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]
public class UserStoreController : ApiControllerBase
{
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(ApiResponse<UserStoreDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserStoreDto>>>> GetByAccountIdAsync(string userId)
    {
        return await Mediator.Send(new GetUserStoreByUserIdQuery { UserId = userId });
    }

    [HttpGet("store-user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<UserStoreDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserStoreDto>>>> GetUserByIdsAsync(string userId)
    {
        return await Mediator.Send(new GetUserIdsForCurrentUserStoreQuery { UserId = userId });
    }
}