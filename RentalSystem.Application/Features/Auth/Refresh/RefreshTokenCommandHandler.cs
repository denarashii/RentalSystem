using MediatR;
using Microsoft.EntityFrameworkCore;
using RentalSystem.Application.Common.Interfaces;
using RentalSystem.Domain.Entities;
using RentalSystem.Domain.Exceptions;

namespace RentalSystem.Application.Features.Auth.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    private readonly IAppDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    public RefreshTokenCommandHandler(
        IAppDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task<RefreshTokenResult> Handle(
        RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var incomingHash = _refreshTokenGenerator.Hash(request.RefreshToken);

        var existingToken = await _context.RefreshTokens
            .Include(rt => rt.User)
                .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(rt => rt.TokenHash == incomingHash, cancellationToken);

        if (existingToken is null)
            throw new InvalidRefreshTokenException();

        // Reuse detection: this exact token was already rotated away once before.
        // Someone is presenting a token that should no longer exist as "current" —
        // treat it as a compromise signal and kill every active token for this user.
        if (existingToken.IsRevoked)
        {
            var allUserTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == existingToken.UserId && rt.RevokedAtUtc == null)
                .ToListAsync(cancellationToken);

            foreach (var token in allUserTokens)
                token.Revoke();

            await _context.SaveChangesAsync(cancellationToken);
            throw new InvalidRefreshTokenException();
        }

        if (existingToken.IsExpired)
            throw new InvalidRefreshTokenException();

        // Rotation: issue a new token, revoke the old one, link them together.
        var (newRawToken, newTokenHash) = _refreshTokenGenerator.Generate();
        var newRefreshToken = RefreshToken.Issue(
            existingToken.UserId, newTokenHash, RefreshTokenLifetime);

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken); // save so newRefreshToken.Id exists

        existingToken.Revoke(newRefreshToken.Id);

        var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(
            existingToken.User, existingToken.User.Role.Name);

        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResult(newAccessToken, newRawToken);
    }
}