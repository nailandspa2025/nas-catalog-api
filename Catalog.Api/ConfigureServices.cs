using Catalog.Infrastructure.Persistence;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Common.Swagger;
using BuildingBlocks.EventBus;

namespace Catalog.Api;

public static class ConfigureServices
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAPIServices();

        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddCustomSwagger(new Version[]
        {
            new Version(1, 0, 0)
        }, nameof(Catalog));

        services.AddHealthChecks()
            .AddDbContextCheck<DbContext>();

        services.AddControllers();
        services.AddCloudinaryProvider(configuration);
        services.AddEventServices(typeof(Program).Assembly, configuration);

        return services;
    }
}

