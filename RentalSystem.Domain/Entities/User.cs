using RentalSystem.Domain.Common;

namespace RentalSystem.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool MustChangePassword { get; private set; }

    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    private User() { } // EF Core

    private User(string email, string passwordHash, string firstName,
                 string lastName, Guid roleId, bool mustChangePassword)
    {
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        RoleId = roleId;
        MustChangePassword = mustChangePassword;
    }

    /// <summary>
    /// Factory method for self-registration (Customer sign-up).
    /// The Application layer resolves "Customer" role id and hashes the password
    /// before calling this — Domain never knows about BCrypt or the DB.
    /// </summary>
    public static User Register(string email, string passwordHash,
                                 string firstName, string lastName, Guid customerRoleId)
    {
        Validate(email, firstName, lastName);
        return new User(email, passwordHash, firstName, lastName,
                         customerRoleId, mustChangePassword: false);
    }

    /// <summary>
    /// Factory method for an Owner/Admin creating a Staff/Admin account.
    /// Forces a password change on first login — see Sprint 1 user flow.
    /// </summary>
    public static User CreateInternal(string email, string tempPasswordHash,
                                       string firstName, string lastName, Guid roleId)
    {
        Validate(email, firstName, lastName);
        return new User(email, tempPasswordHash, firstName, lastName,
                         roleId, mustChangePassword: true);
    }

    private static void Validate(string email, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("A valid email is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.", nameof(lastName));
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        MustChangePassword = false;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("User is already deactivated.");
        IsActive = false;
    }

    public void Reactivate() => IsActive = true;

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = phoneNumber?.Trim();
    }
}