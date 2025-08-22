using System;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ServicePackageConfiguration: IEntityTypeConfiguration<ServicePackage>
{
    public void Configure(EntityTypeBuilder<ServicePackage> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(250)
            .IsRequired(true);

        builder.Property(x => x.Description)
            .HasMaxLength(250)
            .IsRequired(false);
    }
}

