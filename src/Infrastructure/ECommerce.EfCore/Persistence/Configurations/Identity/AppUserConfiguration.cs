using ECommerce.Application.Features.Auth.Constants;
using ECommerce.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.EfCore.Persistence.Configurations.Identity;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("users");

        builder.Property(x => x.FirstName)
            .HasMaxLength(AuthValidationRules.FirstNameMaxLength)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(AuthValidationRules.LastNameMaxLength)
            .IsRequired();

        builder.Property(x => x.RefreshToken)
            .HasMaxLength(500);

        builder.Property(x => x.UserName)
            .HasMaxLength(AuthValidationRules.UserNameMaxLength);

        builder.Property(x => x.NormalizedUserName)
            .HasMaxLength(AuthValidationRules.UserNameMaxLength);

        builder.Property(x => x.Email)
            .HasMaxLength(AuthValidationRules.EmailMaxLength);

        builder.Property(x => x.NormalizedEmail)
            .HasMaxLength(AuthValidationRules.EmailMaxLength);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.HasIndex(x => x.UserName)
            .IsUnique();

        builder.HasIndex(x => x.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName("idx_users_normalized_email");

        builder.HasIndex(x => x.NormalizedUserName)
            .IsUnique()
            .HasDatabaseName("idx_users_normalized_username");
    }
}
