using BuildingBlocks.Core.Response;
using Catalog.Application.Features.ServicePackages.Models;
using Catalog.Application.Features.ServicePackages.Queries.GetServicePackageByStore;
using Catalog.Application.Features.Services.Commands.CreateService;
using Catalog.Application.Features.Services.Commands.DeleteService;
using Catalog.Application.Features.Services.Commands.UpdateService;
using Catalog.Application.Features.Services.Models;
using Catalog.Application.Features.Services.Queries.GetService;
using Catalog.Application.Features.Services.Queries.GetServiceByStoreId;
using Catalog.Application.Features.Services.Queries.GetServices;
using Catalog.Application.Features.Services.Queries.GetServicesWithPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]
public class ServicesController : ApiControllerBase
{
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ServiceDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<ServiceDto>>>> GetWithPaginationAsync([FromQuery] GetServicesWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [AllowAnonymous]
    [HttpGet("mobile-pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ServiceDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<ServiceDto>>>> GetWithPaginationForMobileAsync([FromQuery] GetServicesWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

   
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ServiceDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetServiceByIdQuery { Id = id });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ServiceDto>>> CreateAsync([FromForm] CreateServiceCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ServiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ServiceDto>>> UpdateAsync(int id, [FromForm] UpdateServiceCommand command)
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
        return await Mediator.Send(new DeleteServiceCommand(id));
    }

    [HttpGet("ids")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServiceDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ServiceDto>>>> GetByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetServiceByIdsQuery { Ids = ids });
    }

    [HttpGet("storeId")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServiceDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ServiceDto>>>> GetByStoreIdAsync(long storeId)
    {
        return await Mediator.Send(new GetServiceByStoreIdQuery { StoreId = storeId });
    }

}
