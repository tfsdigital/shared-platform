using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Core.Events;
using Shared.Correlation.Context;
using Shared.Inbox.Resilience;
using Shared.Inbox.Services;
using Shared.Inbox.Settings;
using Shared.Inbox.Storage;
using Shared.Messaging.Connection;
using Shared.Publishing;

namespace Shared.Inbox.Extensions;

public static class InboxExtensions
{
    public static IServiceCollection AddInboxConsumer<TEvent>(
        this IServiceCollection services,
        string moduleName,
        string exchangeName,
        string connectionString,
        int intervalInSeconds,
        int messagesBatchSize)
        where TEvent : IIntegrationEvent
    {
        services.AddHostedService(sp =>
        {
            var settings = new InboxSettings
            {
                ConnectionString = connectionString,
                IntervalInSeconds = intervalInSeconds,
                MessagesBatchSize = messagesBatchSize
            };

            var logger = sp.GetRequiredService<ILogger<InboxIntegrationEventConsumer<TEvent>>>();
            var messageBusFactory = sp.GetRequiredService<IMessageBusConnectionFactory>();

            var inboxStorage = new InboxStorage(settings);

            var consumer = new InboxIntegrationEventConsumer<TEvent>(
                moduleName,
                exchangeName,
                logger,
                messageBusFactory,
                inboxStorage,
                settings
            );

            return consumer;
        });

        return services;
    }

    public static IServiceCollection AddInboxProcessor(
        this IServiceCollection services,
        string moduleName,
        string connectionString,
        int intervalInSeconds,
        int messagesBatchSize)
    {
        services.AddHostedService(sp =>
        {
            var settings = new InboxSettings
            {
                ConnectionString = connectionString,
                IntervalInSeconds = intervalInSeconds,
                MessagesBatchSize = messagesBatchSize
            };

            var resiliencePipeline = InboxResilience.CreateDefault();
            var inboxStorage = new InboxStorage(settings);

            var logger = sp.GetRequiredService<ILogger<InboxProcessorBackgroundService>>();
            var correlationContext = sp.GetRequiredService<ICorrelationContext>();

            var scope = sp.CreateScope();
            var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

            var inboxProcessor = new InboxProcessorBackgroundService(
                moduleName,
                correlationContext,
                eventPublisher,
                logger,
                inboxStorage,
                resiliencePipeline,
                settings);

            return inboxProcessor;
        });

        return services;
    }
}
