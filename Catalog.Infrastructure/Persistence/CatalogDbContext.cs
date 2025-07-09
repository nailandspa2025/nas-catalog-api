using System.Reflection;
using BuildingBlocks.Persistence.EntityFrameworkCore;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public class CatalogDbContext: EfCoreDbContext<CatalogDbContext>, ICatalogDbContext
{
    public CatalogDbContext(
        DbContextOptions<CatalogDbContext> options
        )
        : base(options)
    {
    }

    public DbSet<Product> Product => Set<Product>();

    public DbSet<Store> Store => Set<Store>();

    public DbSet<StoreImageGallery> StoreImageGallery => Set<StoreImageGallery>();

    public DbSet<Banner> Banner => Set<Banner>();

    public DbSet<UserStore> UserStore => Set<UserStore>();

    public DbSet<Calendar> Calendar => Set<Calendar>();

    public DbSet<CalendarType> CalendarType => Set<CalendarType>();

    public DbSet<CalendarOverride> CalendarOverride => Set<CalendarOverride>();

    public DbSet<Merchant> Merchant => Set<Merchant>();

    public DbSet<Brand> Brand => Set<Brand>();

    public DbSet<ServicePackage> ServicePackage =>Set<ServicePackage>();

    public DbSet<Reward> Reward => Set<Reward>();

    public DbSet<ReviewStore> ReviewStore =>Set<ReviewStore>();

    public DbSet<Service> Service => Set<Service>();

    public DbSet<BankAccount> BankAccount => Set<BankAccount>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}

