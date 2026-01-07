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
        var userAgent = Request.Headers["User-Agent"]
            .FirstOrDefault()?
            .ToLowerInvariant() ?? "";

        Console.WriteLine($"UA = {userAgent}");

        var result = await Mediator.Send(new GetAppDeepLinkByCodeQuery
        {
            Code = code
        });

        if (!result.Succeeded || result.Data == null)
            return NotFound(result);

        var dto = result.Data;

        if (IsIos(userAgent))
        {
            return Content(
                GenerateIosHtml(dto.IOSLink, dto.Type),
                "text/html"
            );
        }
        if (IsAndroid(userAgent))
        {
            return Content(
                GenerateAndroidHtml(),
                "text/html"
            );
        }

        return Redirect(dto.WebFallback);
    }
    private static bool IsIos(string ua)
        => ua.Contains("iphone")
        || ua.Contains("ipad")
        || ua.Contains("ipod");
    
    private static bool IsAndroid(string ua)
        => ua.Contains("android");
    
    private static string GenerateIosHtml(string iosLink, string type)
{
    const string merchantAppStore =
        "https://apps.apple.com/us/app/nas-business/id6751517132";
    const string clientAppStore =
        "https://apps.apple.com/us/app/nas-nail-spa/id6746377567";

    var appStoreUrl = type == "merchant"
        ? merchantAppStore
        : clientAppStore;

    return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1'>
<title>Open App</title>

<script>
function openApp() {{
    window.location.href = '{iosLink}';
    setTimeout(function() {{
        window.location.href = '{appStoreUrl}';
    }}, 1500);
}}
</script>
</head>

<body>
    <p>Tap the button to open the app</p>
    <button onclick='openApp()'>Open App</button>
</body>
</html>";
}

    
    private static string GenerateAndroidHtml()
{
    return @"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1'>
<title>Open App</title>

<script>
function openApp() {
    window.location.href = 'market://details?id=com.nas.business';
    setTimeout(function() {
        window.location.href = 'https://play.google.com/store/apps/details?id=com.nas.business';
    }, 1500);
}
</script>
</head>

<body>
    <p>Tap the button to open the app</p>
    <button onclick='openApp()'>Open App</button>
</body>
</html>";
}

}