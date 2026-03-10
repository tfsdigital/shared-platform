using Shared.Core.Events;

namespace Shared.Publishing;

public interface IEventPublisher
{
    Task Publish<TEvent>(TEvent eventBase, CancellationToken cancellationToken = default)
        where TEvent : IEventBase;
}
