using Microsoft.Extensions.DependencyInjection;

namespace Shared.Publishing.Extensions;

public static class PublishingExtensions
{
    public static IServiceCollection AddEventPublishing(this IServiceCollection services)
    {
        return services.AddTransient<IEventPublisher, EventPublisher>();
    }
}
