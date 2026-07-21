using RentalSystem.Domain.Common;

namespace RentalSystem.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; private set; } = null!;

    private Role() { } // EF Core needs a parameterless constructor

    public Role(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.", nameof(name));

        Name = name;
    }

    // Well-known role name constants — avoids "magic strings" scattered
    // across Application/API when checking roles.
    public static class Names
    {
        public const string Owner = "Owner";
        public const string Admin = "Admin";
        public const string Staff = "Staff";
        public const string Customer = "Customer";
    }
}