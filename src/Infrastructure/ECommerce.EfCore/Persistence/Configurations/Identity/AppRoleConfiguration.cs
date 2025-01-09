using ECommerce.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.EfCore.Persistence.Configurations.Identity;

public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.ToTable("roles");

        builder.Property(x => x.Name)
            .HasMaxLength(50);

        builder.Property(x => x.NormalizedName)
            .HasMaxLength(50);

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("idx_roles_name");

        builder.HasIndex(x => x.NormalizedName)
            .IsUnique()
            .HasDatabaseName("idx_roles_normalized_name");
    }
}