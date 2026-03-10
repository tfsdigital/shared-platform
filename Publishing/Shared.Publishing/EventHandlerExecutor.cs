using Shared.Core.Events;

namespace Shared.Publishing;

public record EventHandlerExecutor(
    object HandlerInstance, Func<IEventBase, CancellationToken, Task> HandlerCallback);