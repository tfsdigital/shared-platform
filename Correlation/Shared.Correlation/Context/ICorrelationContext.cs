namespace Shared.Correlation.Context;

public interface ICorrelationContext
{
    string NewCorrelationId();
    string? GetCorrelationId();
    void SetCorrelationId(string correlationId);
}

