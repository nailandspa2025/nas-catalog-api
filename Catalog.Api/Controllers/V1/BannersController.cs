using BuildingBlocks.Core.Response;
using Catalog.Application.Features.Banners.Commands.CreateBanner;
using Catalog.Application.Features.Banners.Commands.DeleteBanner;
using Catalog.Application.Features.Banners.Commands.UpdateBanner;
using Catalog.Application.Features.Banners.Models;
using Catalog.Application.Features.Banners.Queries.GetBanner;
using Catalog.Application.Features.Banners.Queries.GetBannersWithPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]
public class BannersController : ApiControllerBase
{
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<BannerDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<BannerDto>>>> GetBannersWithPaginationAsync([FromQuery] GetBannersWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BannerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BannerDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetBannerByIdQuery { Id = id });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BannerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BannerDto>>> CreateAsync([FromForm] CreateBannerCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BannerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<BannerDto>>> UpdateAsync(long id, [FromForm] UpdateBannerCommand command)
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
        return await Mediator.Send(new DeleteBannerCommand(id));
    }

    [AllowAnonymous]
    [HttpGet("pagingation-mobile")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<BannerDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<BannerDto>>>> GetBannersMobileWithPaginationAsync([FromQuery] GetBannersWithPaginationQuery query)
    {
        query.IsMobile = true;
        return await Mediator.Send(query);
    }
}
