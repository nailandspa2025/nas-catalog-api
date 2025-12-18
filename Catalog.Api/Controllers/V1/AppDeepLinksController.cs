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
            return Content(GenerateIosRedirectHtml(dto.IOSLink, dto.Type), "text/html");
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
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Redirecting...</title>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            text-align: center;
            padding-top: 50px;
            background: #f5f5f5;
        }}
        .loader {{
            border: 3px solid #f3f3f3;
            border-top: 3px solid #007AFF;
            border-radius: 50%;
            width: 30px;
            height: 30px;
            animation: spin 1s linear infinite;
            margin: 20px auto;
        }}
        @keyframes spin {{
            0% {{ transform: rotate(0deg); }}
            100% {{ transform: rotate(360deg); }}
        }}
    </style>
</head>
<body>
    <div class='loader'></div>
    <p>Opening app...</p>
    
    <script>
        (function() {{
            var deepLink = '{iosLink}';
            var appStoreUrl = '{appStoreUrl}';
            var timeout = null;
            var startTime = Date.now();
            
            // Tạo iframe ẩn để thử mở deep link
            var iframe = document.createElement('iframe');
            iframe.style.display = 'none';
            iframe.src = deepLink;
            document.body.appendChild(iframe);
            
            // Đồng thời thử mở bằng window.location
            window.location.href = deepLink;
            
            // Fallback: nếu sau 2s vẫn còn ở trang này thì chuyển App Store
            timeout = setTimeout(function() {{
                // Kiểm tra nếu trang vẫn visible (app không mở được)
                if (!document.hidden) {{
                    window.location.href = appStoreUrl;
                }}
            }}, 2000);
            
            // Nếu app mở được, page sẽ bị hide -> clear timeout
            document.addEventListener('visibilitychange', function() {{
                if (document.hidden) {{
                    clearTimeout(timeout);
                }}
            }});
            
            window.addEventListener('pagehide', function() {{
                clearTimeout(timeout);
            }});
            
            window.addEventListener('blur', function() {{
                clearTimeout(timeout);
            }});
        }})();
    </script>
</body>
</html>";
    }
}