namespace ECommerce.Application.Common.Settings;

public sealed record JwtSettings
{
    public const string SectionName = "JwtSettings";
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string SecretKey { get; init; } = null!;
    public int ExpirationInMinutes { get; init; }
    public int RefreshTokenExpirationInDays { get; init; }
}