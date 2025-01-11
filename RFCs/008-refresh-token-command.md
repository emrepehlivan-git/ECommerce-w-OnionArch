# RFC 008: Refresh Token Command Implementation

## Status

Proposed

## Context

Access token'ın süresi dolduğunda yeni bir token almak için refresh token kullanılması gerekiyor. Bu RFC, RefreshTokenCommand'in implementasyonunu detaylandırır.

## Detailed Design

### Command Structure

```csharp
public sealed record RefreshTokenCommand(
    string RefreshToken) : IRequest<Result<AuthResponseDto>>, IValidatableRequest;

public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User);
```

### Command Handler

```csharp
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IJwtService _jwtService;
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenCommandHandler(
        IJwtService jwtService,
        UserManager<AppUser> userManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

        if (user is null)
            return Result<AuthResponseDto>.NotFound("Invalid refresh token");

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Result<AuthResponseDto>.Invalid(new[] { new ValidationError("Refresh token expired") });

        var userDto = new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.UserName);

        var (accessToken, refreshToken) = _jwtService.GenerateTokens(userDto);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

        // Update refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = expiresAt.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);

        var response = new AuthResponseDto(
            accessToken,
            refreshToken,
            expiresAt,
            userDto);

        return Result<AuthResponseDto>.Success(response);
    }
}
```

### Validation Rules

```csharp
public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MinimumLength(64) // Base64 encoded 32 bytes
            .MaximumLength(128);
    }
}
```

## Implementation Details

### Dependencies

- MediatR
- FluentValidation
- ASP.NET Core Identity
- JWT Authentication
- Ardalis.Result

### Validation Pipeline

Command, `IValidatableRequest` interface'ini implement eder ve `ValidationBehavior` pipeline'ından geçer.

### Error Handling

1. Validation Errors

   - Empty refresh token
   - Invalid token format

2. Business Logic Errors
   - Token not found
   - Token expired
   - User not found
   - Token revoked

### Success Response

- New access token
- New refresh token
- Token expiration time
- User details

### Error Response

- Invalid token errors
- Token expiration errors
- User status errors

## Security Considerations

1. Token Security

   - One-time use refresh tokens
   - Token rotation on each use
   - Secure token storage
   - Token revocation capability

2. Token Validation

   - Expiration time check
   - Token format validation
   - User account status check

3. Attack Prevention
   - Rate limiting
   - Token reuse detection
   - Suspicious activity monitoring

## Testing Strategy

1. Unit Tests

   - Command validation
   - Handler logic
   - Token generation
   - Error scenarios

2. Integration Tests

   - Token refresh flow
   - Token rotation
   - Expiration handling

3. Security Tests
   - Token reuse attempts
   - Invalid token handling
   - Concurrent refresh attempts

## Performance Considerations

1. Database

   - Token indexing
   - Async operations
   - Efficient queries

2. Token Generation
   - Efficient JWT creation
   - Minimal claims payload
   - Optimized crypto operations

## Monitoring

1. Metrics

   - Refresh success rate
   - Token expiration rate
   - Average refresh time
   - Reuse attempts

2. Logging

   - Failed refresh attempts
   - Token generation errors
   - Suspicious patterns

3. Alerts
   - Multiple failed attempts
   - Token reuse attempts
   - Unusual refresh patterns

## References

1. [JWT Best Practices](https://datatracker.ietf.org/doc/html/rfc7519)
2. [OAuth 2.0 Refresh Token](https://datatracker.ietf.org/doc/html/rfc6749#section-1.5)
3. [Token Security](https://auth0.com/docs/secure/tokens)

```

```
