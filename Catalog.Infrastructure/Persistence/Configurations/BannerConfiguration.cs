using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configuration;

public class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.Property(p => p.Title)
            .HasMaxLength(150);

        builder.Property(p => p.Link)
            .HasMaxLength(350);

        builder.HasMany(x => x.ImageGallerys)
            .WithOne(x => x.Banner)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
    }
}

