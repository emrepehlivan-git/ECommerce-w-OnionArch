namespace ECommerce.Application.Features.Auth.Dtos;
public sealed record UserDto(Guid Id, string FirstName, string LastName, string Email, string UserName);
