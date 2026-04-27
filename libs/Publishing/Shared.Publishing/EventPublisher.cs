using Shared.Events;
using System.Collections.Concurrent;

namespace Shared.Publishing;

public class EventPublisher(IServiceProvider serviceProvider) : IEventPublisher
{
    private static readonly ConcurrentDictionary<Type, IEventHandlerWrapper> _eventHandlers = new();

    public Task Publish<TEvent>(TEvent eventBase, CancellationToken cancellationToken = default)
        where TEvent : IEventBase
    {
        ArgumentNullException.ThrowIfNull(eventBase);

        var handler = _eventHandlers.GetOrAdd(
            eventBase.GetType(),
            static eventType =>
            {
                var wrapperType = typeof(EventHandlerWrapperImplementation<>).MakeGenericType(
                    eventType
                );
                var wrapper =
                    Activator.CreateInstance(wrapperType)
                    ?? throw new InvalidOperationException(
                        $"Could not create wrapper for type {eventType}"
                    );

                return (IEventHandlerWrapper)wrapper;
            }
        );

        return handler.Handle(eventBase, serviceProvider, PublishCore, cancellationToken);
    }

    protected static async Task PublishCore(
        IEnumerable<EventHandlerExecutor> handlerExecutors,
        IEventBase eventBase,
        CancellationToken cancellationToken
    )
    {
        foreach (var handler in handlerExecutors)
        {
            await handler.HandlerCallback(eventBase, cancellationToken).ConfigureAwait(false);
        }
    }
}
