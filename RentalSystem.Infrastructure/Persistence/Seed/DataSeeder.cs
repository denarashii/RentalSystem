using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RentalSystem.Application.Common.Interfaces;
using RentalSystem.Domain.Entities;
using RentalSystem.Infrastructure.Persistence.Configurations;

namespace RentalSystem.Infrastructure.Persistence.Seed;

public static class DataSeeder
{
    public static async Task SeedInitialOwnerAsync(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        ILogger logger)
    {
        // Guard: never re-seed if an Owner already exists. This makes the
        // seeder safe to call on every app startup, not just the first time.
        var ownerExists = await context.Users
            .AnyAsync(u => u.RoleId == RoleConfiguration.OwnerRoleId);

        if (ownerExists)
        {
            logger.LogInformation("Owner account already exists — skipping seed.");
            return;
        }

        var email = configuration["InitialOwner:Email"]
            ?? throw new InvalidOperationException("InitialOwner:Email is not configured.");
        var password = configuration["InitialOwner:Password"]
            ?? throw new InvalidOperationException("InitialOwner:Password is not configured.");

        var passwordHash = passwordHasher.Hash(password);

        var owner = User.CreateInternal(
            email: email,
            tempPasswordHash: passwordHash,
            firstName: "System",
            lastName: "Owner",
            roleId: RoleConfiguration.OwnerRoleId);

        context.Users.Add(owner);
        await context.SaveChangesAsync();

        logger.LogInformation("Initial Owner account seeded for {Email}", email);
    }
}