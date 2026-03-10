# Publishing

Domain event publishing. Collects domain events from aggregates (via ChangeTracker) and dispatches them to handlers.

## Architecture

- `IEventPublisher`: Publishes domain events
- `IEventHandlerWrapper`: Wraps `IEventHandler<T>` for MediatR or similar
- Used by DbContext in `SaveChangesAsync` to publish events after saving

## Main Abstractions

```csharp
public interface IEventPublisher
{
    Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : IEventBase;
}
```

## Usage Example

Typically invoked from DbContext override:

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
{
    var domainEvents = ChangeTracker.Entries<AggregateRoot>()
        .SelectMany(e => e.Entity.DomainEvents)
        .ToList();

    foreach (var evt in domainEvents)
        await _eventPublisher.PublishAsync(evt, ct);

    foreach (var entry in ChangeTracker.Entries<AggregateRoot>())
        entry.Entity.ClearEvents();

    return await base.SaveChangesAsync(ct);
}
```

Handlers (`IEventHandler<TEvent>`) are invoked by the publisher. Register handlers in DI; `AddHandlersFromAssembly` discovers them.
