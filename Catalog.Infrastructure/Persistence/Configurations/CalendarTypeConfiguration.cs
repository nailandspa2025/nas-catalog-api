using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class CalendarTypeConfiguration : IEntityTypeConfiguration<CalendarType>
{
    public void Configure(EntityTypeBuilder<CalendarType> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(p => p.Color)
           .HasMaxLength(100);
    }
}