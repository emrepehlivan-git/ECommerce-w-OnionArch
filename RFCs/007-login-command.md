# RFC 007: Login Command Implementation

## Status

Proposed

## Context

Kullanıcı girişi işleminin CQRS pattern'i kullanılarak gerçekleştirilmesi gerekiyor. Bu RFC, LoginCommand'in implementasyonunu detaylandırır.

## Detailed Design

### Command Structure

```csharp
public sealed record LoginCommand(
    string Email,
    string Password,
    bool RememberMe = false) : IRequest<Result<AuthResponseDto>>, IValidatableRequest;

public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User);
```

### Command Handler

```csharp
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        UserManager<AppUser> userManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result<AuthResponseDto>.NotFound("User not found");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return Result<AuthResponseDto>.Invalid(new[] { new ValidationError("Invalid credentials") });

        var userDto = new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.UserName);

        var (accessToken, refreshToken) = _jwtService.GenerateTokens(userDto);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

        // Save refresh token
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
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(AuthValidationRules.EmailMaxLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(AuthValidationRules.PasswordMinLength)
            .MaximumLength(AuthValidationRules.PasswordMaxLength);
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

   - Boş alan kontrolleri
   - Email format kontrolü
   - Password length kontrolü

2. Business Logic Errors
   - User not found
   - Invalid password
   - Account locked/disabled

### Success Response

- Access token
- Refresh token
- Token expiration time
- User details

### Error Response

- Invalid credentials
- Account status errors
- Validation errors

## Security Considerations

1. Authentication Security

   - JWT token encryption
   - Refresh token rotation
   - Password attempt limiting

2. Token Security

   - Short-lived access tokens
   - Secure token storage
   - Token revocation capability

3. Session Management
   - Remember me functionality
   - Concurrent session handling
   - Session termination

## Testing Strategy

1. Unit Tests

   - Command validation
   - Handler logic
   - Token generation
   - Error scenarios

2. Integration Tests
   - Authentication flow
   - Token validation
   - Refresh token mechanism

## Performance Considerations

1. Database

   - Email indexing
   - Async operations
   - Caching user data

2. Token Generation
   - Efficient JWT creation
   - Minimal claims payload

## Monitoring

1. Metrics

   - Login success rate
   - Failed attempts
   - Token refresh rate
   - Average login time

2. Logging

   - Failed login attempts
   - Token generation errors
   - Suspicious activities

3. Alerts
   - Multiple failed attempts
   - Unusual login patterns
   - Token validation failures

## References

1. [JWT Best Practices](https://datatracker.ietf.org/doc/html/rfc7519)
2. [OAuth 2.0 Refresh Token](https://datatracker.ietf.org/doc/html/rfc6749#section-1.5)
3. [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)

```

```
