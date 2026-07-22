namespace RentalSystem.Application.Common.Interfaces;

public interface IRefreshTokenGenerator
{
    (string RawToken, string TokenHash) Generate();
    string Hash(string rawToken);
}