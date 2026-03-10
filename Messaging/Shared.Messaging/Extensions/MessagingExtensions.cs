using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging.Abstractions;
using Shared.Messaging.Connection;

namespace Shared.Messaging.Extensions;

public static class MessagingExtensions
{
    public static IServiceCollection AddMessagingServices(this IServiceCollection services)
    {
        services.AddScoped<IMessageBus, MessageBus>();
        services.AddSingleton<IMessageBusConnectionFactory, MessageBusConnectionFactory>();

        return services;
    }
}
