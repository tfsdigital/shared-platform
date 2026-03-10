using System.Diagnostics;
using Shared.Core.Identification;

namespace Shared.Correlation.Context;

public class CorrelationContext : ICorrelationContext
{
    private readonly AsyncLocal<string> _correlationId = new();

    public string NewCorrelationId()
    {
        var correlationId = IdGenerator.CreateSequential().ToString("N");

        SetCorrelationId(correlationId);

        return correlationId;
    }

    public string? GetCorrelationId()
    {
        return _correlationId.Value;
    }

    public void SetCorrelationId(string correlationId)
    {
        if (string.IsNullOrWhiteSpace(correlationId))
            throw new ArgumentNullException(nameof(correlationId));

        _correlationId.Value = correlationId;

        Activity.Current?.SetTag("correlation_id", correlationId);
    }
}

