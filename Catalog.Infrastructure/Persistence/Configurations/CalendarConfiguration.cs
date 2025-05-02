using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations
{
    public class CalendarConfiguration: IEntityTypeConfiguration<Domain.Entities.Calendar>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Calendar> builder)
        {
            builder.Property(p => p.Title)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(250);

            builder.HasOne(x => x.CalendarType)
                .WithMany(x => x.Calendars)
                .HasForeignKey(bg => bg.CalendarTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.HasMany(x => x.CalendarOverrides)
                .WithOne(x => x.Calendar)
                .HasForeignKey(bg => bg.CalendarId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

        }
    }
}

