using BuildingBlocks.Core.Response;
using Catalog.Application.Features.AppDeepLinks.Commands.AddAppDeepLink;
using Catalog.Application.Features.AppDeepLinks.Commands.CreateAppDeepLink;
using Catalog.Application.Features.AppDeepLinks.Models;
using Catalog.Application.Features.AppDeepLinks.Queries.GetAppDeepLink;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[ApiVersion("1.0")]
public class AppDeepLinksController : ApiControllerBase
{
    [HttpGet("detail")]
    [ProducesResponseType(typeof(ApiResponse<AppDeepLinkDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AppDeepLinkDto>>> GetDetailAsync([FromQuery] GetAppDeepLinkByTypeTargetIdQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AppDeepLinkDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AppDeepLinkDto>>> CreateAsync([FromForm] CreateAppDeepLinkCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("assign-store")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> AddAsync([FromBody] AddAppDeepLinkCommand command)
    {
        return await Mediator.Send(command);
    }

    [AllowAnonymous]
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(ApiResponse<AppDeepLinkDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCodeAsync(string code)
    {
        var result = await Mediator.Send(new GetAppDeepLinkByCodeQuery { Code = code });
        if (!result.Succeeded)
            return NotFound(result);

        var dto = result.Data;
        var userAgent = Request.Headers["User-Agent"].ToString().ToLower();
        if (IsIosDevice(userAgent))
        {
            return Content(GenerateIosRedirectHtml(dto.IOSLink), "text/html");
        }

        var redirectUrl = IsAndroidDevice(userAgent) ? dto.AndroidLink : dto.WebFallback;
        return Redirect(redirectUrl);
    }

    private bool IsAndroidDevice(string userAgent)
    {
        return userAgent.Contains("android");
    }

    private bool IsIosDevice(string userAgent)
    {
        return userAgent.Contains("iphone") || userAgent.Contains("ipad") || userAgent.Contains("ios");
    }

    private string GenerateIosRedirectHtml(string iosLink)
    {
        const string appStoreUrl = "https://apps.apple.com/us/app/nas-nail-spa/id6746377567";
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>Redirecting...</title>
            <script type='text/javascript'>
                window.onload = function() {{
                    window.location = '{iosLink}';
                    setTimeout(function() {{
                        window.location = '{appStoreUrl}';
                    }}, 2000);
                }};
            </script>
        </head>
        <body>
            <p>Loading ...</p>
        </body>
        </html>";
    }
}