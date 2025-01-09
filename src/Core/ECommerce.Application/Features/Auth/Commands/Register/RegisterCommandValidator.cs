using FluentValidation;
using ECommerce.Application.Features.Auth.Constants;
using ECommerce.Application.Common.Interfaces;

namespace ECommerce.Application.Features.Auth.Commands.Register;

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