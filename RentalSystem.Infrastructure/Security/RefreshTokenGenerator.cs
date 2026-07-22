using System.Security.Cryptography;
using RentalSystem.Application.Common.Interfaces;

namespace RentalSystem.Infrastructure.Security;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public (string RawToken, string TokenHash) Generate()
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var tokenHash = Hash(rawToken);
        return (rawToken, tokenHash);
    }

    public string Hash(string rawToken)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToBase64String(bytes);
    }
}