using RentalSystem.Domain.Entities;

namespace RentalSystem.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user, string roleName);
}