namespace Huntly.Core.Shared.Entities;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    protected void UpdateTimestamp() => UpdatedAt = DateTime.UtcNow;
}