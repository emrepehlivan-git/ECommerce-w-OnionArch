using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Domain.Identity;

public sealed class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public AppUser()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
    }
}