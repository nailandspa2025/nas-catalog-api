using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.ProductName)
            .HasMaxLength(250)
            .IsRequired();
<<<<<<< HEAD

        builder.HasMany(x => x.Stores)
            .WithMany(x => x.Products);
=======
>>>>>>> 3399663 (product)
    }
}

