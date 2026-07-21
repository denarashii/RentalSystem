namespace RentalSystem.Domain.Common;

/// <summary>
/// Base class for all entities that need identity + audit tracking.
/// Lives in Domain (not Shared) because these are core business concerns —
/// "when was this created, who touched it last" is a domain concept,
/// not a technical cross-cutting one.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    public bool IsDeleted => DeletedAtUtc.HasValue;

    public void MarkUpdated(Guid? updatedBy)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SoftDelete(Guid? deletedBy)
    {
        if (IsDeleted) return;
        DeletedAtUtc = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }
}