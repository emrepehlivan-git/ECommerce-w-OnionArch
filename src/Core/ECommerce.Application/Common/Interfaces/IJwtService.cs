using ECommerce.Application.Features.Auth.Dtos;

namespace ECommerce.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(UserDto user);
    UserDto? ValidateToken(string token);
    string GenerateRefreshToken();
    (string token, string refreshToken) GenerateTokens(UserDto user);
    Task<bool> ValidateRefreshToken(string refreshToken, Guid userId, CancellationToken cancellationToken = default);
}