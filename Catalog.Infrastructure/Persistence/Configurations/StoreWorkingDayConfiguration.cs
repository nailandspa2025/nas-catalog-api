using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configuration;
public class StoreWorkingDayConfiguration : IEntityTypeConfiguration<StoreWorkingDay>
{
    public void Configure(EntityTypeBuilder<StoreWorkingDay> builder)
    {
        builder.HasOne(x => x.Store)
            .WithMany(s => s.StoreWorkingDays)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.DayOfWeek)
            .IsRequired();
    }
}