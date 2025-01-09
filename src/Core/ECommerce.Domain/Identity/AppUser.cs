using Microsoft.AspNetCore.Identity;

namespace ECommerce.Domain.Identity;

public sealed class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public AppUser()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
    }
}