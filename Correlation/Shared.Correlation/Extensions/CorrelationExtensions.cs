using Microsoft.Extensions.DependencyInjection;
using Shared.Correlation.Context;

namespace Shared.Correlation.Extensions;

public static class CorrelationExtensions
{
    public static IServiceCollection AddCorrelationContext(this IServiceCollection services)
    {
        services.AddSingleton<ICorrelationContext, CorrelationContext>();

        return services;
    }

    public static IDictionary<string, string> ToHeaders(this ICorrelationContext correlationContext)
    {
        var correlationId = correlationContext.GetCorrelationId();

        if (correlationId == null)
            throw new InvalidOperationException("Correlation id is empty");

        return new Dictionary<string, string>
        {
            ["correlation_id"] = correlationId
        };
    }
}
