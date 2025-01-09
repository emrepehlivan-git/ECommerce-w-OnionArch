using ECommerce.Domain.Entities;
using ECommerce.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AppUser> Users { get; }
    DbSet<AppRole> Roles { get; }
    DbSet<AppUserClaim> UserClaims { get; }
    DbSet<AppUserRole> UserRoles { get; }
    DbSet<AppUserLogin> UserLogins { get; }
    DbSet<AppUserToken> UserTokens { get; }
    DbSet<AppRoleClaim> RoleClaims { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

