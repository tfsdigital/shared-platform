namespace Shared.Core.Events;

public interface IEventBase
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}
