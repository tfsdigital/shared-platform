using System.Collections.Concurrent;
using Shared.Core.Events;

namespace Shared.Publishing;

public class EventPublisher(IServiceProvider serviceProvider) : IEventPublisher
{
    private static readonly ConcurrentDictionary<Type, IEventHandlerWrapper> EventHandlers = new();

    public Task Publish<TEvent>(TEvent eventBase, CancellationToken cancellationToken = default)
        where TEvent : IEventBase
    {
        ArgumentNullException.ThrowIfNull(eventBase);

        var handler = EventHandlers.GetOrAdd(eventBase.GetType(), static eventType =>
        {
            var wrapperType = typeof(EventHandlerWrapperImplementation<>).MakeGenericType(eventType);
            var wrapper = Activator.CreateInstance(wrapperType) ??
                throw new InvalidOperationException($"Could not create wrapper for type {eventType}");

            return (IEventHandlerWrapper)wrapper;
        });

        return handler.Handle(eventBase, serviceProvider, PublishCore, cancellationToken);
    }

    protected static async Task PublishCore(
        IEnumerable<EventHandlerExecutor> handlerExecutors,
        IEventBase eventBase,
        CancellationToken cancellationToken)
    {
        foreach (var handler in handlerExecutors)
        {
            await handler.HandlerCallback(eventBase, cancellationToken).ConfigureAwait(false);
        }
    }
}
