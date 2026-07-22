using MediatR;

namespace RentalSystem.Application.Features.Auth.Logout;

public record LogoutCommand(string RefreshToken) : IRequest;