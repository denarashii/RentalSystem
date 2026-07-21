using MediatR;

namespace RentalSystem.Application.Features.Auth.Register;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<RegisterUserResult>;

public record RegisterUserResult(Guid UserId, string Email);