using Shared.Core.Events;

namespace Shared.Core.Domain;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public DateTime? RemovedAt { get; private set; }
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public virtual void Remove() => RemovedAt = DateTime.UtcNow;

    protected void RaiseEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearEvents() => _domainEvents.Clear();
}
