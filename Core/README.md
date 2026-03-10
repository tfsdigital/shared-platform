# Core

Domain base classes, events, and identification utilities.

## Architecture

Shared.Core is referenced by all contexts. Provides: `AggregateRoot`, `DomainEvent`, `IntegrationEvent`, `IEventHandler`, `IdGenerator`.

## Main Abstractions

```csharp
// Aggregate root
public abstract class AggregateRoot
{
    public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    protected void RaiseEvent(IDomainEvent domainEvent);
    public void ClearEvents();
}

// Domain event
public abstract record DomainEvent : IDomainEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
}

// Integration event
public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; }
    public DateTime OccurredOn { get; init; }
}

// Event handler
public interface IEventHandler<in TEvent> where TEvent : IEventBase
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

// ID generation
IdGenerator.CreateSequential()
```

## Usage Example

```csharp
public class Tab : AggregateRoot
{
    public void Close()
    {
        if (Status == TabStatus.Closed) return;
        Status = TabStatus.Closed;
        RaiseEvent(new TabClosedDomainEvent(this));
    }
}

public record TabClosedDomainEvent(Tab Tab) : DomainEvent;

public class TabClosedDomainEventHandler : IEventHandler<TabClosedDomainEvent>
{
    public async Task HandleAsync(TabClosedDomainEvent e, CancellationToken ct)
    {
        // Handle event
    }
}
```
