using MediatR;
using Microsoft.EntityFrameworkCore;
using RentalSystem.Application.Common.Interfaces;
using RentalSystem.Domain.Entities;
using RentalSystem.Domain.Exceptions;

namespace RentalSystem.Application.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    public LoginCommandHandler(
        IAppDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        // Deliberately identical failure for "no such user" and "wrong password" —
        // never reveal which one it was, or you hand attackers a way to enumerate
        // valid emails one guess at a time.
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        if (!user.IsActive)
            throw new AccountDeactivatedException();

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, user.Role.Name);
        var (rawRefreshToken, refreshTokenHash) = _refreshTokenGenerator.Generate();

        var refreshTokenEntity = RefreshToken.Issue(user.Id, refreshTokenHash, RefreshTokenLifetime);
        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResult(
            AccessToken: accessToken,
            RefreshToken: rawRefreshToken, // the RAW token goes to the client — only the hash is stored
            UserId: user.Id,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Role: user.Role.Name,
            MustChangePassword: user.MustChangePassword
        );
    }
}