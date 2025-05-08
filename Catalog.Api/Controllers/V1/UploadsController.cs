using BuildingBlocks.Core.Response;
using Catalog.Application.Features.Uploads.Commands.CreateUpload;
using Catalog.Application.Features.Uploads.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class UploadsController : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UploadDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<UploadDto>>> CreateAsync([FromForm] CreateUploadCommand command)
        {
            return await Mediator.Send(command);
        }
    }
}
