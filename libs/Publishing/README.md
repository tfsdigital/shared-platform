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

Handlers (`IEventHandler<TEvent>`) are invoked by the publisher. Register handlers in DI; `AddHandlersFromAssembly` discovers them.
