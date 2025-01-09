using ECommerce.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.EfCore.Persistence.Configurations.Identity;

public class AppUserLoginConfiguration : IEntityTypeConfiguration<AppUserLogin>
{
    public void Configure(EntityTypeBuilder<AppUserLogin> builder)
    {
        builder.ToTable("user_logins");

        builder.HasKey(x => new { x.LoginProvider, x.ProviderKey });

        // Index for foreign key
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("idx_user_logins_user_id");
    }
}