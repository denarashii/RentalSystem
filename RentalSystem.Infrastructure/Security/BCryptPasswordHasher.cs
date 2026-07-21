using RentalSystem.Application.Common.Interfaces;

namespace RentalSystem.Infrastructure.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    // Work factor 12 is a reasonable default in 2026 — high enough to resist
    // brute force, low enough to not noticeably slow down login requests.
    private const int WorkFactor = 12;

    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}