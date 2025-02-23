using BuildingBlocks.MultiTenancy.Abstractions;
using BuildingBlocks.Persistence.Abstractions.Auditing;
using BuildingBlocks.Persistence.Abstractions.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Persistence.Extensions
{
    public static class EntityTypeBuilderExtensions
	{
        public static void ConfigureByConvention(this EntityTypeBuilder b)
        {
            b.TryConfigureSoftDelete();
            b.TryConfigureAudited();
            b.TryConfigureMultiTenant();
        }

        public static void TryConfigureSoftDelete(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo(typeof(ISoftDelete)))
            {
                b.Property<bool>(nameof(ISoftDelete.IsDeleted))
                    .IsRequired()
                    .HasAnnotation("Relational:DefaultValue", false)
                    .HasAnnotation("Relational:ColumnName", nameof(ISoftDelete.IsDeleted));

                b.Property<DateTime?>(nameof(ISoftDelete.Deleted))
                    .IsRequired(false)
                    .HasAnnotation("Relational:ColumnName", nameof(ISoftDelete.Deleted));

                b.Property<string>(nameof(ISoftDelete.DeletedBy))
                    .HasMaxLength(255)
                    .IsRequired(false)
                    .HasAnnotation("Relational:ColumnName", nameof(ISoftDelete.DeletedBy));
            }
        }


        public static void TryConfigureAudited(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo(typeof(IAuditable)))
            {
                b.Property(nameof(IAuditable.Created))
                    .IsRequired()
                    .HasAnnotation("Relational:ColumnName", nameof(IAuditable.Created));

                b.Property(nameof(IAuditable.CreatedBy))
                    .HasMaxLength(255)
                    .IsRequired(false)
                   .HasAnnotation("Relational:ColumnName", nameof(IAuditable.CreatedBy));

                b.Property(nameof(IAuditable.LastModified))
                    .IsRequired(false)
                    .HasAnnotation("Relational:ColumnName", nameof(IAuditable.LastModified));

                b.Property(nameof(IAuditable.LastModifiedBy))
                    .HasMaxLength(255)
                    .IsRequired(false)
                   .HasAnnotation("Relational:ColumnName", nameof(IAuditable.LastModifiedBy));
            }
        }

        public static void ConfigureMultiTenant<T>(this EntityTypeBuilder<T> b)
            where T : class, IMultiTenant
        {
            b.TryConfigureMultiTenant();
        }

        public static void TryConfigureMultiTenant(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo(typeof(IMultiTenant)))
            {
                b.Property(nameof(IMultiTenant.TenantId))
                    .IsRequired(false)
                    .HasAnnotation("Relational:ColumnName", nameof(IMultiTenant.TenantId));
            }
        }
    }
}

