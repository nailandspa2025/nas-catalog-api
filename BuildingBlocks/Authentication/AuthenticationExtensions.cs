using BuildingBlocks.Authentication.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Authentication;

public static class AuthenticationExtensions
{
    public static void AddCustomAuthentication(this IServiceCollection services)
    {
        services.AddSingleton<ICurrentUser, CurrentUser>();
        services.AddHttpContextAccessor();

        var jwtOptions = new JwtBearerOptions();
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
        configuration?.GetSection("Jwt").Bind(jwtOptions);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = jwtOptions.Authority;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
            });
    }
}

