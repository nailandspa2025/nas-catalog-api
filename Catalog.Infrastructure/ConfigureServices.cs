using Catalog.Application.Common.Interfaces;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Services;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Persistence;
using BuildingBlocks.Persistence.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.ApiClients;
using BuildingBlocks.ApiClients.Clients.Order;
using BuildingBlocks.ApiClients.Extensions;
using Refit;
using BuildingBlocks.ApiClients.Clients.Identity;

namespace Catalog.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.Is("UseInMemoryDatabase"))
        {
            services.AddCustomDbContext<CatalogDbContext>(configuration, EfCoreDatabaseProvider.InMemory);
        }
        else
        {
            services.AddCustomDbContext<CatalogDbContext>(configuration, EfCoreDatabaseProvider.PostgreSql);
        }

        services.AddScoped<ICatalogDbContext>(provider => provider.GetRequiredService<CatalogDbContext>());
        services.AddScoped<CatalogDbContextInitialiser>();

        services.AddTransient<IDateTime, DateTimeService>();

        services.AddScoped<AuthorizationMessageHandler>();
        services.AddRefitClient<IOrderClient>()
               .ConfigureHttpClient((sp, client) => client.BaseAddress = configuration
                                                                       .GetSection("MicroserviceUri")
                                                                       .GetValue<Uri>("OrderAddress"))
                .AddHttpMessageHandler<AuthorizationMessageHandler>();

        services.AddRefitClient<IIdentityClient>()
               .ConfigureHttpClient((sp, client) => client.BaseAddress = configuration
                                                                       .GetSection("MicroserviceUri")
                                                                       .GetValue<Uri>("IdentityAddress"))
               .AddHttpMessageHandler<AuthorizationMessageHandler>();

        return services;
    }
}