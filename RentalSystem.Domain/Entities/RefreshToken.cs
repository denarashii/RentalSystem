using RentalSystem.Domain.Common;

namespace RentalSystem.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    /// <summary>
    /// SHA-256 hash of the actual token — the raw token is only ever
    /// returned to the client once, at issuance, and never persisted.
    /// </summary>
    public string TokenHash { get; private set; } = null!;

    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }

    /// <summary>
    /// Points to the token that replaced this one when rotated.
    /// Lets us detect reuse of an already-rotated token (see below).
    /// </summary>
    public Guid? ReplacedByTokenId { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;

    private RefreshToken() { } // EF Core

    private RefreshToken(Guid userId, string tokenHash, DateTime expiresAtUtc)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
    }

    /// <summary>
    /// Factory for issuing a brand-new token at login.
    /// The Application layer generates the raw token + computes the hash —
    /// Domain never sees the raw value, only the hash it's asked to store.
    /// </summary>
    public static RefreshToken Issue(Guid userId, string tokenHash, TimeSpan lifetime)
        => new(userId, tokenHash, DateTime.UtcNow.Add(lifetime));

    /// <summary>
    /// Marks this token as revoked and links it to whatever token replaced it.
    /// Called during rotation: old token dies, new token is issued.
    /// </summary>
    public void Revoke(Guid? replacedByTokenId = null)
    {
        if (IsRevoked) return;
        RevokedAtUtc = DateTime.UtcNow;
        ReplacedByTokenId = replacedByTokenId;
    }
}