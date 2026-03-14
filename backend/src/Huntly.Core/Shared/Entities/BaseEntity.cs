namespace Huntly.Core.Shared.Entities;

public abstract class BaseEntity
{
    // Guid v7 is time-ordered — avoids index fragmentation on inserts vs random v4
    public Guid Id { get; protected set; } = Guid.CreateVersion7();
}