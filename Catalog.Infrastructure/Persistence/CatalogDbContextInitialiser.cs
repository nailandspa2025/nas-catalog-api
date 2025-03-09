using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
=======
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
>>>>>>> 3399663 (product)
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Persistence;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<CatalogDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}


public class CatalogDbContextInitialiser
{
    private readonly ILogger<CatalogDbContextInitialiser> _logger;
    private readonly CatalogDbContext _context;

    public CatalogDbContextInitialiser(ILogger<CatalogDbContextInitialiser> logger, CatalogDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (!_context.Database.IsInMemory())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        await Task.CompletedTask;
    }
}

<<<<<<< HEAD
=======
public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        string projectRoot = Path.Combine(Directory.GetCurrentDirectory(), "../Catalog.API");
        if (!Directory.Exists(projectRoot))
        {
            throw new Exception($"API project directory not found: {projectRoot}");
        }
        var configuration = new ConfigurationBuilder()
            .SetBasePath(projectRoot)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception($"ConnectionString not found in appsettings.{environment}.json");
        }
        var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new CatalogDbContext(optionsBuilder.Options);
    }
}

>>>>>>> 3399663 (product)
