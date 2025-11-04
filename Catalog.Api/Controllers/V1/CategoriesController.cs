using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Catalog.Application.Features.Categories.Commands.CreateCategory;
using Catalog.Application.Features.Categories.Commands.DeleteCategory;
using Catalog.Application.Features.Categories.Commands.UpdateCategory;
using Catalog.Application.Features.Categories.Models;
using Catalog.Application.Features.Categories.Queries.GetCategories;
using Catalog.Application.Features.Categories.Queries.GetCategory;
using Catalog.Application.Features.Categories.Queries.GetCategoryWithPagination;
using Catalog.Application.Features.Stores.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]
public class CategoriesController: ApiControllerBase
{
    [AccessGroup("category.view")]
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<CategoryDto>>>> GetWithPaginationAsync([FromQuery] GetCategoryWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [AccessGroup("category.view")]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetCategoryByIdQuery { Id = id });
    }

    [AccessGroup("category.view")]
    [HttpGet("ids")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetCategoryByIdsQuery { Ids = ids });
    }

    [AccessGroup("category.delete")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(int id)
    {
        return await Mediator.Send(new DeleteCategoryCommand(id));
    }

    [AccessGroup("category.create")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateAsync([FromForm] CreateCategoryCommand command)
    {
        return await Mediator.Send(command);
    }

    [AccessGroup("category.update")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<StoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateAsync(int id, [FromForm] UpdateCategoryCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        return await Mediator.Send(command);
    }

    //Mobile
    [AllowAnonymous]
    [HttpGet("mobile-pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<CategoryDto>>>> GetWithPaginationForMobileAsync([FromQuery] GetCategoryWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }
}

