using BuildingBlocks.Persistence.EntityFrameworkCore;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Common.Interfaces;


public interface ICatalogDbContext : IEfCoreDbContext
{
    DbSet<Product> Product { get; }

    DbSet<Store> Store { get; }

    DbSet<StoreImageGallery> StoreImageGallery { get; }

    DbSet<Banner> Banner { get; }

    DbSet<UserStore> UserStore { get; }

    DbSet<Calendar> Calendar { get; }

    DbSet<CalendarType> CalendarType { get; }

    DbSet<CalendarOverride> CalendarOverride { get; }

    DbSet<Merchant> Merchant { get; }

    DbSet<Brand> Brand { get; }

    DbSet<ServicePackage> ServicePackage { get; }

    DbSet<Reward> Reward { get; }

    DbSet<ReviewStore> ReviewStore { get; }

    DbSet<Service> Service { get; }

    DbSet<BankAccount> BankAccount { get; }

    DbSet<SocialNetwork> SocialNetwork { get; }

    DbSet<AppDeepLink> AppDeepLink { get; }

    DbSet<UserStoreDeepLink> UserStoreDeepLink { get; }

    DbSet<ReviewTechnician> ReviewTechnician { get; }

    DbSet<ReviewService> ReviewService { get; }

    DbSet<Category> Category { get; }

}

