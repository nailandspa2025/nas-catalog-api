using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class PaypalConfigConfiguration : IEntityTypeConfiguration<PayPalConfig>
{
    public void Configure(EntityTypeBuilder<PayPalConfig> builder)
    {
        builder.HasOne(s => s.Store)
            .WithOne(pc => pc.PayPalConfig)
            .HasForeignKey<PayPalConfig>(pc => pc.StoreId);
    }
}
