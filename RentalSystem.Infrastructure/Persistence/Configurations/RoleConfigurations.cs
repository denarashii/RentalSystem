using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalSystem.Domain.Entities;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public static readonly Guid OwnerRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid AdminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid StaffRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid CustomerRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(r => r.Name).IsUnique();

        // Seed data — HasData works against the *shadow state* of the entity,
        // so it bypasses our factory methods/constructors entirely. That's why
        // we configure Id and Name directly here via an anonymous object,
        // not by calling `new Role(...)`.
        builder.HasData(
            new { Id = OwnerRoleId, Name = Role.Names.Owner, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = AdminRoleId, Name = Role.Names.Admin, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = StaffRoleId, Name = Role.Names.Staff, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = CustomerRoleId, Name = Role.Names.Customer, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}