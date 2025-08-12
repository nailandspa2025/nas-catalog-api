using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class UserStoreDeepLinkConfiguration : IEntityTypeConfiguration<UserStoreDeepLink>
{
    public void Configure(EntityTypeBuilder<UserStoreDeepLink> builder)
    {

        builder.HasOne(x => x.Store)
            .WithMany(x => x.UserStoreDeepLinks)
            .HasForeignKey(bg => bg.StoreId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}
