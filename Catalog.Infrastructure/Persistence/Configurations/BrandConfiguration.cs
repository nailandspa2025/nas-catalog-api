using System;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class BrandConfiguration: IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.Property(x => x.Name)
             .HasMaxLength(50)
             .IsRequired();

        builder.Property(x => x.Description)
             .HasMaxLength(250)
             .IsRequired();

        builder.Property(x => x.Logo)
             .HasMaxLength(250)
             .IsRequired();

        builder.HasMany(x => x.Stores)
            .WithOne(x => x.Brand)
            .HasForeignKey(bg => bg.BrandId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

