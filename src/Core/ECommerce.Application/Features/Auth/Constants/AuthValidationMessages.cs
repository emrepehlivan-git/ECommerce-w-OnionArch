namespace ECommerce.Application.Features.Auth.Constants;

public static class AuthValidationMessages
{
    public const string RequireUppercase = "Password must contain at least one uppercase letter";
    public const string RequireLowercase = "Password must contain at least one lowercase letter";
    public const string RequireNumber = "Password must contain at least one number";
    public const string MismatchConfirmation = "Passwords do not match";
    public const string EmailAlreadyExists = "Email already exists";
    public const string UserNameAlreadyExists = "Username already exists";
}