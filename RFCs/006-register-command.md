# RFC 006: Register Command Implementation

## Status

Implemented

## Context

Kullanıcı kayıt işleminin CQRS pattern'i kullanılarak gerçekleştirilmesi gerekiyor. Bu RFC, RegisterCommand'in implementasyonunu detaylandırır.

## Detailed Design

### Command Structure

```csharp
public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string Password,
    string ConfirmPassword) : IRequest<Result<Guid>>, IValidatableRequest;
```

### Command Handler

```csharp
public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Guid>>
{
    private readonly UserManager<AppUser> _userManager;

    public RegisterCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.UserName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => new ValidationError
                {
                    Identifier = nameof(request.Password),
                    ErrorMessage = e.Description
                })
                .ToList();
            return Result<Guid>.Invalid(errors);
        }

        return Result<Guid>.Success(user.Id);
    }
}
```

### Validation Rules

```csharp
public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(AuthValidationRules.FirstNameMinLength)
            .MaximumLength(AuthValidationRules.FirstNameMaxLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(AuthValidationRules.LastNameMinLength)
            .MaximumLength(AuthValidationRules.LastNameMaxLength);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(AuthValidationRules.EmailMaxLength);

        RuleFor(x => x.UserName)
            .NotEmpty()
            .MinimumLength(AuthValidationRules.UserNameMinLength)
            .MaximumLength(AuthValidationRules.UserNameMaxLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(AuthValidationRules.PasswordMinLength)
            .MaximumLength(AuthValidationRules.PasswordMaxLength)
            .Matches("[A-Z]").WithMessage(AuthValidationMessages.RequireUppercase)
            .Matches("[a-z]").WithMessage(AuthValidationMessages.RequireLowercase)
            .Matches("[0-9]").WithMessage(AuthValidationMessages.RequireNumber);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage(AuthValidationMessages.MismatchConfirmation);
    }
}
```

## Implementation Details

### Dependencies

- MediatR
- FluentValidation
- ASP.NET Core Identity
- Ardalis.Result

### Validation Pipeline

Command, `IValidatableRequest` interface'ini implement eder ve `ValidationBehavior` pipeline'ından geçer.

### Error Handling

1. Validation Errors

   - Boş alan kontrolleri
   - Minimum/Maximum uzunluk kontrolleri
   - Email format kontrolü
   - Parola karmaşıklık kontrolü
   - Parola eşleşme kontrolü

2. Business Logic Errors
   - Duplicate email
   - Duplicate username
   - Password complexity requirements

### Success Response

- Kullanıcı başarıyla oluşturulduğunda kullanıcının ID'si döner
- Response tipi: `Result<Guid>`

### Error Response

- Validation hatalarında `Result.Invalid` döner
- Her hata için `ValidationError` objesi oluşturulur

## Security Considerations

1. Password Security

   - Minimum 6 karakter
   - En az bir büyük harf
   - En az bir küçük harf
   - En az bir rakam
   - Özel karakter opsiyonel

2. Data Protection
   - Email ve username unique olmalı
   - Parola hash'lenerek saklanır
   - Sensitive data loglanmaz

## Testing Strategy

1. Unit Tests

   - Command validation
   - Handler logic
   - Error scenarios

2. Integration Tests
   - Database operations
   - Duplicate checks
   - Password hashing

## Performance Considerations

1. Database

   - Email ve username için index
   - Async operations

2. Validation
   - Early validation ile gereksiz DB çağrılarını engelleme

## Monitoring

1. Metrics

   - Registration success rate
   - Validation failure rate
   - Average registration time

2. Logging
   - Failed registrations (without sensitive data)
   - Validation errors
   - Performance metrics

## References

1. [MediatR Documentation](https://github.com/jbogard/MediatR/wiki)
2. [FluentValidation Documentation](https://docs.fluentvalidation.net/en/latest/)
3. [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)

```

```
