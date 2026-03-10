using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Events;

namespace Shared.Publishing;

public class EventHandlerWrapperImplementation<TEvent> : IEventHandlerWrapper
    where TEvent : IEventBase
{
    public Task Handle(IEventBase eventBase, IServiceProvider serviceProvider,
        Func<IEnumerable<EventHandlerExecutor>, IEventBase, CancellationToken, Task> publish,
        CancellationToken cancellationToken)
    {
        var handlers = serviceProvider
            .GetServices<IEventHandler<TEvent>>()
            .Select(static handler => new EventHandlerExecutor(
                HandlerInstance: handler,
                HandlerCallback: (eventToHandle, token) =>
                    handler.HandleAsync((TEvent)eventToHandle, token)
                ));

        return publish(handlers, eventBase, cancellationToken);
    }
}
