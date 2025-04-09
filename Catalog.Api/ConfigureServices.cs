using Catalog.Infrastructure.Persistence;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Common.Swagger;
using BuildingBlocks.EventBus;
using BuildingBlocks.CommonAuthorization.CommonAuthorizationExtensions;

namespace Catalog.Api;

public static class ConfigureServices
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAPIServices(configuration);

        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddCustomSwagger(new Version[]
        {
            new Version(1, 0, 0)
        }, nameof(Catalog));

        services.AddHealthChecks()
            .AddDbContextCheck<CatalogDbContext>();

        services.AddControllers();
        services.AddCloudinaryProvider(configuration);
        services.AddEventServices(typeof(Program).Assembly, configuration);
        services.AddCommonAuthorization();

        return services;
    }
}

