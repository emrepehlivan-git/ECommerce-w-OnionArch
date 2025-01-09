using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.EfCore.Persistence;

public class ApplicationDbContext : IdentityDbContext<
    AppUser, AppRole, Guid,
    AppUserClaim, AppUserRole, AppUserLogin,
    AppRoleClaim, AppUserToken>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
