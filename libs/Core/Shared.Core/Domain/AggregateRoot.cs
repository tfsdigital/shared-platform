namespace Shared.Core.Domain;

public abstract class AggregateRoot
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public DateTime? RemovedAt { get; private set; }

    public virtual void Remove() => RemovedAt = DateTime.UtcNow;
}
