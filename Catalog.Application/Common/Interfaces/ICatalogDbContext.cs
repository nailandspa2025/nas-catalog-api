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
}

