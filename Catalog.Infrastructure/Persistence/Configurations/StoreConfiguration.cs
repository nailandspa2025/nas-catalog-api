using System;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class StoreConfiguration: IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.Property(p => p.StoreName)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(p => p.AddressStore)
            .HasMaxLength(500);

        builder.Property(p => p.Hotline)
            .HasMaxLength(20);

        builder.HasMany(x => x.ImageGallerys)
            .WithOne(x => x.Store)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
    }
}
