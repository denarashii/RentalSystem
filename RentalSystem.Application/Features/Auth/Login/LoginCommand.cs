using MediatR;

namespace RentalSystem.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public record LoginResult(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool MustChangePassword
);