using Shared.Core.Events;

namespace Shared.Publishing;

public interface IEventHandlerWrapper
{
    Task Handle(IEventBase eventBase, IServiceProvider serviceProvider,
        Func<IEnumerable<EventHandlerExecutor>, IEventBase, CancellationToken, Task> publish,
        CancellationToken cancellationToken);
}
