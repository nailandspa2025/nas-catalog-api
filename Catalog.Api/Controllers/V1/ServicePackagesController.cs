using BuildingBlocks.Core.Response;
using Catalog.Application.Features.ServicePackages.Commands.CreateServicePackage;
using Catalog.Application.Features.ServicePackages.Commands.DeleteServicePackage;
using Catalog.Application.Features.ServicePackages.Commands.UpdateServicePackage;
using Catalog.Application.Features.ServicePackages.Models;
using Catalog.Application.Features.ServicePackages.Queries.GetServicePackage;
using Catalog.Application.Features.ServicePackages.Queries.GetServicePackageByStore;
using Catalog.Application.Features.ServicePackages.Queries.GetServicePackages;
using Catalog.Application.Features.ServicePackages.Queries.GetServicePackagesWithPagination;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]
public class ServicePackagesController : ApiControllerBase
{
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ServicePackageDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<ServicePackageDto>>>> GetWithPaginationAsync([FromQuery] GetServicePackagesWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }


    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ServicePackageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ServicePackageDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetServicePackageByIdQuery { Id = id });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ServicePackageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ServicePackageDto>>> CreateAsync([FromForm] CreateServicePackageCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ServicePackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ServicePackageDto>>> UpdateAsync(int id, [FromForm] UpdateServicePackageCommand command)
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
        return await Mediator.Send(new DeleteServicePackageCommand(id));
    }

    [HttpGet("ids")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServicePackageDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ServicePackageDto>>>> GetByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetServicePackageByIdsQuery { Ids = ids });
    }

    [HttpGet("storeId")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServicePackageDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ServicePackageDto>>>> GetByStoreIdAsync(long storeId)
    {
        return await Mediator.Send(new GetServicePackageByStoreIdQuery { StoreId = storeId });
    }
}
