
using BuildingBlocks.Core.Response;
using Catalog.Application.Features.AppDeepLinks.Models;
using Catalog.Application.Features.AppDeepLinks.Queries.GetAppDeepLink;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;
[Route("")]
[ApiController]
public class DeepLinkRedirectController : ControllerBase
{
    private readonly IMediator _mediator;
    public DeepLinkRedirectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{code}")]
    [ProducesResponseType(typeof(ApiResponse<AppDeepLinkDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AppDeepLinkDto>>> GetByCodeAsync(string code)
    {
        var result = await _mediator.Send(new GetAppDeepLinkByCodeQuery { Code = code });
        if (!result.Succeeded) return NotFound(result);
        var dto = result.Data;
        var userAgent = Request.Headers["User-Agent"].ToString().ToLower();
        var redirectUrl = dto.WebFallback;
        if (userAgent.Contains("iphone") || userAgent.Contains("ipad") || userAgent.Contains("ios"))
            redirectUrl = dto.IOSLink;
        else if (userAgent.Contains("android"))
            redirectUrl = dto.AndroidLink;
        return Redirect(redirectUrl);
    }
}