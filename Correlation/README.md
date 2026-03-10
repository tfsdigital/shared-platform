# Correlation

Correlation ID for distributed tracing across requests and services.

## Architecture

- **Shared.Correlation**: `ICorrelationContext` with `AsyncLocal` storage
- **Shared.Correlation.OpenTelemetry**: Log processor that injects correlation ID into OpenTelemetry log records

## Main Abstractions

```csharp
public interface ICorrelationContext
{
    string NewCorrelationId();
    string? GetCorrelationId();
    void SetCorrelationId(string correlationId);
}

// Extension to convert to HTTP headers
correlationContext.ToHeaders() // => { ["correlation_id"] = "..." }
```

## Usage Example

```csharp
// Registration
services.AddCorrelationContext();

// Middleware: create or propagate correlation ID from header
var correlationId = context.Request.Headers["correlation_id"].FirstOrDefault()
    ?? correlationContext.NewCorrelationId();
correlationContext.SetCorrelationId(correlationId);
context.Response.Headers["correlation_id"] = correlationId;

// Anywhere in the request pipeline
var id = correlationContext.GetCorrelationId(); // Available in logs via OpenTelemetry processor

// Outgoing HTTP
request.Headers.Add(correlationContext.ToHeaders());
```
