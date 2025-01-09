using Ardalis.Result;
using ECommerce.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Features.Auth.Commands.Register;

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