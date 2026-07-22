using MediatR;

namespace RentalSystem.Application.Features.Auth.Refresh;

public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenResult>;

public record RefreshTokenResult(string AccessToken, string RefreshToken);