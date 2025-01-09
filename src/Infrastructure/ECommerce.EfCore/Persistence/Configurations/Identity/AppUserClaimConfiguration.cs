using ECommerce.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.EfCore.Persistence.Configurations.Identity;

public class AppUserClaimConfiguration : IEntityTypeConfiguration<AppUserClaim>
{
    public void Configure(EntityTypeBuilder<AppUserClaim> builder)
    {
        builder.ToTable("user_claims");

        // Index for foreign key
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("idx_user_claims_user_id");

        // Index for claim type searches
        builder.HasIndex(x => x.ClaimType)
            .HasDatabaseName("idx_user_claims_claim_type");
    }
}