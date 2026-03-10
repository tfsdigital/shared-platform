using Shared.Core.Identification;

namespace Shared.Core.Events;

public abstract record DomainEvent : IDomainEvent
{
    public Guid Id { get; } = IdGenerator.CreateSequential();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
