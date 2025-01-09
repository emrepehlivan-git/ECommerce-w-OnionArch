using ECommerce.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.EfCore.Persistence.Configurations.Identity;

public class AppRoleClaimConfiguration : IEntityTypeConfiguration<AppRoleClaim>
{
    public void Configure(EntityTypeBuilder<AppRoleClaim> builder)
    {
        builder.ToTable("role_claims");

        // Index for foreign key
        builder.HasIndex(x => x.RoleId)
            .HasDatabaseName("idx_role_claims_role_id");

        // Index for claim type searches
        builder.HasIndex(x => x.ClaimType)
            .HasDatabaseName("idx_role_claims_claim_type");
    }
}