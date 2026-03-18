namespace Shared.Core.Events;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; }
    public DateTime OccurredOnUtc { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public string? CausationId { get; init; }
    public string Source { get; init; } = string.Empty;
}
