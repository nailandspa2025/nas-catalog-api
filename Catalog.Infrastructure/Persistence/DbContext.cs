using System.Reflection;
using Catalog.Application.Common.Interfaces;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.MultiTenancy.Abstractions;
using BuildingBlocks.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public class DbContext: EfCoreDbContext<DbContext>, IDbContext
{
    public DbContext(
        DbContextOptions<DbContext> options,
        ICurrentUser currentUser,
        ITenantResolver tenantResolver)
        : base(options, currentUser, tenantResolver)
    {
    }

    //public DbSet<Catalog.Domain.Entities.Booking> Booking => Set<Catalog.Domain.Entities.Booking>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}

