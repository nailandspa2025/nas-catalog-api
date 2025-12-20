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
        var userAgent = Request.Headers["User-Agent"].FirstOrDefault();
        userAgent = userAgent?.ToLowerInvariant() ?? "";

        Console.WriteLine($"UA: {userAgent}");

        var result = await Mediator.Send(new GetAppDeepLinkByCodeQuery { Code = code });
        if (!result.Succeeded)
            return NotFound(result);

        var dto = result.Data;
        if (IsIosDevice(userAgent))
        {
            return Content(GenerateIosRedirectHtml(dto.IOSLink, dto.Type), "text/html");
        }

        var redirectUrl = IsAndroidDevice(userAgent) ? dto.AndroidLink : dto.WebFallback;
        if (string.IsNullOrWhiteSpace(redirectUrl))
        {
            return BadRequest("Redirect URL is empty");
        }

        Console.WriteLine($"redirectUrl: {redirectUrl}");

        return Redirect(redirectUrl);
    }

    private static bool IsAndroidDevice(string ua)
    {
         Console.WriteLine($"UA: {ua.Contains("android")}");
        return ua.Contains("android");
    }

    private static bool IsIosDevice(string ua)
    {
        return ua.Contains("iphone")
        || ua.Contains("ipad")
        || ua.Contains("ipod");
    }

    private string GenerateIosRedirectHtml(string iosLink, string type)
    {

        const string merchantUrl = "https://apps.apple.com/us/app/nas-business/id6751517132";
        const string clientUrl = "https://apps.apple.com/us/app/nas-nail-spa/id6746377567";

        string appStoreUrl = type == "merchant" ? merchantUrl : clientUrl;

        return $@"
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset='UTF-8'>
        <title>Redirecting...</title>

        <!-- iOS camera mở link sẽ ưu tiên meta-refresh -->
        <meta http-equiv='refresh' content='0; url={iosLink}' />

        <script>
            // Safari mở trực tiếp vẫn chạy JS
            setTimeout(function() {{
                window.location = '{appStoreUrl}';
            }}, 1500);
        </script>

        <style>
            body {{
                font-family: Arial;
                text-align: center;
                padding-top: 40px;
            }}
        </style>
        </head>
        <body>
            <p>Loading...</p>
        </body>
        </html>";
    }
}