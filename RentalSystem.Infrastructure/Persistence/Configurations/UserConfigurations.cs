using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalSystem.Domain.Entities;

namespace RentalSystem.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        // Unique email, but only among non-deleted users — lets a soft-deleted
        // user's email be reused by a new registration later.
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("\"DeletedAtUtc\" IS NULL");

        builder.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict); // never cascade-delete a Role out from under Users

        // Global query filter — every query against Users automatically
        // excludes soft-deleted rows, so nobody has to remember to add
        // `.Where(u => !u.IsDeleted)` on every single query in the app.
        builder.HasQueryFilter(u => u.DeletedAtUtc == null);
    }
}