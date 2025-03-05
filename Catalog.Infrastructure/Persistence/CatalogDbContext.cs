using System.Reflection;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Persistence.EntityFrameworkCore;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public class CatalogDbContext: EfCoreDbContext<CatalogDbContext>, ICatalogDbContext
{
    public CatalogDbContext(
        DbContextOptions<CatalogDbContext> options,
        ICurrentUser currentUser
        )
        : base(options, currentUser)
    {
    }

    public DbSet<Product> Product => Set<Product>();

    public DbSet<Store> Store => Set<Store>();

    public DbSet<StoreImageGallery> StoreImageGallery => Set<StoreImageGallery>();

    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}

