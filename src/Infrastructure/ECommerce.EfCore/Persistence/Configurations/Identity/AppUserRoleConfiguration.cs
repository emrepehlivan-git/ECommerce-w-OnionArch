using ECommerce.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.EfCore.Persistence.Configurations.Identity;

public class AppUserRoleConfiguration : IEntityTypeConfiguration<AppUserRole>
{
    public void Configure(EntityTypeBuilder<AppUserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(x => new { x.UserId, x.RoleId });

        // Indexes for foreign keys
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("idx_user_roles_user_id");

        builder.HasIndex(x => x.RoleId)
            .HasDatabaseName("idx_user_roles_role_id");
    }
}