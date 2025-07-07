using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class MerchantConfiguration: IEntityTypeConfiguration<Merchant>
{
    public void Configure(EntityTypeBuilder<Merchant> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.ShortName)
            .HasMaxLength(50);

        builder.Property(x => x.ContractNumber)
            .HasMaxLength(50);

        builder.Property(x => x.ZaloOA)
            .HasMaxLength(150);

        builder.Property(x => x.Fanpage)
            .HasMaxLength(150);

        builder.Property(x => x.Website)
            .HasMaxLength(150);

        builder.Property(x => x.Address)
            .HasMaxLength(250);

        builder.Property(x => x.Represent)
           .HasMaxLength(50);

        builder.Property(x => x.Email)
           .HasMaxLength(50);

        builder.Property(x => x.PhoneNumber)
           .HasMaxLength(20);

        builder.Property(x => x.Logo)
           .HasMaxLength(250);

        builder.HasMany(x => x.Brands)
            .WithOne(x => x.Merchant)
            .HasForeignKey(bg => bg.MerchantId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasMany(x => x.MerchantContractImages)
            .WithOne(x => x.Merchant)
            .HasForeignKey(bg => bg.MerchantId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasMany(x => x.Stores)
            .WithOne(x => x.Merchant)
            .HasForeignKey(bg => bg.MerchantId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}