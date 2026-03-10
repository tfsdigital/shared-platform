namespace Shared.Core.Events;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; }
    public DateTime OccurredOn { get; init; }
}