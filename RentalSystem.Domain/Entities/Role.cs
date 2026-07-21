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

    public static class WellKnownIds
    {
        public static readonly Guid Owner = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid Admin = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid Staff = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public static readonly Guid Customer = Guid.Parse("44444444-4444-4444-4444-444444444444");
    }
}