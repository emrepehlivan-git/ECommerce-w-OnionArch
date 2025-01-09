using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Settings;
using ECommerce.Application.Features.Auth.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Infrastructure.Services;

public sealed class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IApplicationDbContext _dbContext;
    public JwtService(IOptions<JwtSettings> jwtSettings, IApplicationDbContext dbContext)
    {
        _jwtSettings = jwtSettings.Value;
        _dbContext = dbContext;
    }

    public string GenerateToken(UserDto user)
    {
        var claims = GetClaims(user);
        var securityKey = CreateSecurityKey();
        var credentials = CreateSigningCredentials(securityKey);
        var expirationTime = DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expirationTime,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string token, string refreshToken) GenerateTokens(UserDto user)
    {
        return (GenerateToken(user), GenerateRefreshToken());
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<bool> ValidateRefreshToken(string refreshToken, Guid userId, CancellationToken cancellationToken = default)
    {
        var refreshTokenEntity = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken && x.UserId == userId, cancellationToken);
        return refreshTokenEntity is not null && refreshTokenEntity.ExpiresAt > DateTime.Now;
    }

    public UserDto? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validatedToken = ValidateJwtToken(tokenHandler, token);
            var jwtToken = (JwtSecurityToken)validatedToken;

            return ExtractUserFromToken(jwtToken);
        }
        catch
        {
            return null;
        }
    }

    private SecurityToken ValidateJwtToken(JwtSecurityTokenHandler tokenHandler, string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = CreateSecurityKey(),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            ClockSkew = TimeSpan.Zero
        };

        tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
        return validatedToken;
    }

    private UserDto ExtractUserFromToken(JwtSecurityToken jwtToken)
    {
        var claimValues = new Dictionary<string, string>
        {
            { JwtRegisteredClaimNames.Sub, GetClaimValue(jwtToken, JwtRegisteredClaimNames.Sub) },
            { JwtRegisteredClaimNames.Email, GetClaimValue(jwtToken, JwtRegisteredClaimNames.Email) },
            { JwtRegisteredClaimNames.Name, GetClaimValue(jwtToken, JwtRegisteredClaimNames.Name) },
            { JwtRegisteredClaimNames.GivenName, GetClaimValue(jwtToken, JwtRegisteredClaimNames.GivenName) },
            { JwtRegisteredClaimNames.FamilyName, GetClaimValue(jwtToken, JwtRegisteredClaimNames.FamilyName) }
        };

        return new UserDto(
            Guid.Parse(claimValues[JwtRegisteredClaimNames.Sub]),
            claimValues[JwtRegisteredClaimNames.GivenName],
            claimValues[JwtRegisteredClaimNames.FamilyName],
            claimValues[JwtRegisteredClaimNames.Email],
            claimValues[JwtRegisteredClaimNames.Name]
        );
    }

    private string GetClaimValue(JwtSecurityToken token, string claimType)
    {
        return token.Claims.First(x => x.Type == claimType).Value;
    }

    private SymmetricSecurityKey CreateSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
    }

    private SigningCredentials CreateSigningCredentials(SecurityKey securityKey)
    {
        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    private Claim[] GetClaims(UserDto user)
    {
        return
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName)
        ];
    }
}