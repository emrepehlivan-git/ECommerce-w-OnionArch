using Ardalis.Result;
using ECommerce.Application.Common.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string Password,
    string ConfirmPassword) : IRequest<Result<Guid>>, IValidatableRequest;