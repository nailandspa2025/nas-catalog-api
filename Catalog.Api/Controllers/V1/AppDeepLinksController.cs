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
        var ua = Request.Headers["User-Agent"].ToString().ToLower();

        bool isIos = IsIosDevice(ua);
        bool isAndroid = IsAndroidDevice(ua);
        bool isZalo = ua.Contains("zalo");

        // ---------- iOS ----------
        if (isIos)
        {
            // ✅ Zalo → auto open scheme
            if (isZalo)
            {
                return Content(
                    GenerateIosAutoRedirectHtml(dto.IOSLink, dto.Type),
                    "text/html"
                );
            }

            // ❗ Safari / Chrome → user click
            return Content(
                GenerateIosManualOpenHtml(dto.IOSLink, dto.Type),
                "text/html"
            );
        }

        // ---------- Android ----------
        if (isAndroid)
        {
            return Redirect(dto.AndroidLink);
        }

        // ---------- Desktop / other ----------
        return Redirect(dto.WebFallback);
    }

    private bool IsAndroidDevice(string userAgent)
    {
        return userAgent.Contains("android");
    }

    private bool IsIosDevice(string userAgent)
    {
        return userAgent.Contains("iphone")
            || userAgent.Contains("ipad")
            || userAgent.Contains("ios");
    }

    private string GenerateIosAutoRedirectHtml(string iosLink, string type)
    {
        string appStoreUrl = GetAppStoreUrl(type);

        return $@"
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset='utf-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1'>
        <title>Opening app...</title>
        </head>
        <body style='font-family:-apple-system;text-align:center;padding-top:60px'>
            <p>Đang mở ứng dụng…</p>

        <script>
        (function() {{
            var deepLink = '{iosLink}';
            var appStore = '{appStoreUrl}';

            // Zalo cho phép auto open
            window.location.href = deepLink;

            // fallback App Store
            setTimeout(function() {{
                window.location.href = appStore;
            }}, 2000);
        }})();
        </script>
        </body>
        </html>";
    }


    private string GenerateIosManualOpenHtml(string iosLink, string type)
    {
        string appStoreUrl = GetAppStoreUrl(type);

        return $@"
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset='utf-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1'>
        <title>Mở ứng dụng</title>
        </head>
        <body style='
            font-family:-apple-system;
            text-align:center;
            padding:40px;
            background:#f5f5f5
        '>
            <h3>NAS App</h3>

            <button
                style='
                    padding:14px 28px;
                    font-size:16px;
                    border:none;
                    border-radius:8px;
                    background:#007AFF;
                    color:white;
                    cursor:pointer'
                onclick=""window.location.href='{iosLink}'"">
                🚀 Mở App
            </button>

            <p style='margin-top:20px'>
                Chưa cài app?
                <br/>
                <a href='{appStoreUrl}' style='color:#007AFF'>
                    Tải từ App Store
                </a>
            </p>
        </body>
        </html>";
    }

    private string GetAppStoreUrl(string type)
    {
        const string merchantUrl =
            "https://apps.apple.com/us/app/nas-business/id6751517132";

        const string clientUrl =
            "https://apps.apple.com/us/app/nas-nail-spa/id6746377567";

        return type == "merchant" ? merchantUrl : clientUrl;
    }



    // [AllowAnonymous]
    // [HttpGet("{code}")]
    // [ProducesResponseType(typeof(ApiResponse<AppDeepLinkDto>), StatusCodes.Status200OK)]
    // public async Task<IActionResult> GetByCodeAsync(string code)
    // {
    //     var result = await Mediator.Send(new GetAppDeepLinkByCodeQuery { Code = code });
    //     if (!result.Succeeded)
    //         return NotFound(result);

    //     var dto = result.Data;
    //     var userAgent = Request.Headers["User-Agent"].ToString().ToLower();
    //     if (IsIosDevice(userAgent))
    //     {
    //         return Content(GenerateIosRedirectHtml(dto.IOSLink, dto.Type), "text/html");
    //     }

    //     var redirectUrl = IsAndroidDevice(userAgent) ? dto.AndroidLink : dto.WebFallback;
    //     return Redirect(redirectUrl);
    // }

    // private bool IsAndroidDevice(string userAgent)
    // {
    //     return userAgent.Contains("android");
    // }

    // private bool IsIosDevice(string userAgent)
    // {
    //     return userAgent.Contains("iphone") || userAgent.Contains("ipad") || userAgent.Contains("ios");
    // }

    // private string GenerateIosRedirectHtml(string iosLink, string type)
    // {

    //     const string merchantUrl = "https://apps.apple.com/us/app/nas-business/id6751517132";
    //     const string clientUrl = "https://apps.apple.com/us/app/nas-nail-spa/id6746377567";

    //     string appStoreUrl = type == "merchant" ? merchantUrl : clientUrl;

    //     return $@"
    //     <!DOCTYPE html>
    //     <html>
    //     <head>
    //     <meta charset='UTF-8'>
    //     <title>Redirecting...</title>

    //     <!-- iOS camera mở link sẽ ưu tiên meta-refresh -->
    //     <meta http-equiv='refresh' content='0; url={iosLink}' />

    //     <script>
    //         // Safari mở trực tiếp vẫn chạy JS
    //         setTimeout(function() {{
    //             window.location = '{appStoreUrl}';
    //         }}, 1500);
    //     </script>

    //     <style>
    //         body {{
    //             font-family: Arial;
    //             text-align: center;
    //             padding-top: 40px;
    //         }}
    //     </style>
    //     </head>
    //     <body>
    //         <p>Loading...</p>
    //     </body>
    //     </html>";
    // }
}