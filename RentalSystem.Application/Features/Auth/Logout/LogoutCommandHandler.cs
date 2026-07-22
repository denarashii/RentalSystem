using MediatR;
using Microsoft.EntityFrameworkCore;
using RentalSystem.Application.Common.Interfaces;

namespace RentalSystem.Application.Features.Auth.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IAppDbContext _context;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    public LogoutCommandHandler(IAppDbContext context, IRefreshTokenGenerator refreshTokenGenerator)
    {
        _context = context;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var incomingHash = _refreshTokenGenerator.Hash(request.RefreshToken);

        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == incomingHash, cancellationToken);

        // Deliberately silent no-op if the token doesn't exist or is already revoked —
        // logout should always succeed from the client's perspective. There's no
        // meaningful security benefit to telling a client "that token wasn't valid
        // anyway" on logout, and it avoids leaking any info about token state.
        if (token is not null && !token.IsRevoked)
        {
            token.Revoke();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}