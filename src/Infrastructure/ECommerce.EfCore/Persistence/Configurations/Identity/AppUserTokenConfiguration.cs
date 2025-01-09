using ECommerce.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.EfCore.Persistence.Configurations.Identity;

public class AppUserTokenConfiguration : IEntityTypeConfiguration<AppUserToken>
{
    public void Configure(EntityTypeBuilder<AppUserToken> builder)
    {
        builder.ToTable("user_tokens");

        builder.HasKey(x => new { x.UserId, x.LoginProvider, x.Name });

        // Index for foreign key
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("idx_user_tokens_user_id");
    }
}