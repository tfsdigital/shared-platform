# Inbox

Idempotent consumption of integration events. Prevents duplicate processing when the same message is delivered more than once.

## Architecture

Consumers extend `InboxEventConsumer<TEvent, TDbConnectionFactory>`. Messages are stored in an inbox table before processing. If the message ID already exists, it is skipped (idempotency).

## Main Abstractions

```csharp
public interface IInboxStorage  // Insert, mark processed, check if exists
```

Base consumer pattern: check inbox → insert if new → process → mark processed.

## Usage Example

```csharp
public class NotificationSentEventConsumer : InboxEventConsumer<NotificationSentIntegrationEvent, INotificationsDbConnectionFactory>
{
    protected override async Task Handle(NotificationSentIntegrationEvent evt, CancellationToken ct)
    {
        // Business logic: send emails, push notifications, etc.
        await _notificationService.SendAsync(evt.Subject, evt.Message, evt.Recipients, ct);
    }
}
```

Registration is typically done via MassTransit or similar. The base class handles idempotency; override `Handle` with your logic.
