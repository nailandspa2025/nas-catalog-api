using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.RateLimiting;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddBuildingBlocksRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RateLimitOptions>(
            configuration.GetSection(RateLimitOptions.SectionName));

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            var serviceProvider = services.BuildServiceProvider();

            var rateLimitOptions =
                serviceProvider
                    .GetRequiredService<IOptions<RateLimitOptions>>()
                    .Value;

            if (!rateLimitOptions.Enabled)
            {
                return;
            }

            AddPolicy(
                options,
                RateLimitPolicies.PublicApi,
                rateLimitOptions.PublicApi);

            AddPolicy(
                options,
                RateLimitPolicies.Login,
                rateLimitOptions.Login);

            AddPolicy(
                options,
                RateLimitPolicies.Admin,
                rateLimitOptions.Admin);
        });

        return services;
    }

    private static void AddPolicy(
        RateLimiterOptions options,
        string policyName,
        PolicyOptions policy)
    {
        options.AddPolicy(
            policyName,
            context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: GetPartitionKey(context),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = policy.PermitLimit,
                        Window = TimeSpan.FromSeconds(policy.WindowSeconds),
                        QueueLimit = policy.QueueLimit,
                        QueueProcessingOrder =
                            QueueProcessingOrder.OldestFirst
                    }));
    }

    private static string GetPartitionKey(
        HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString()
               ?? "unknown";
    }
}