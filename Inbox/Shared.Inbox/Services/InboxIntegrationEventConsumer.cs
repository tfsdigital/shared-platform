using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Core.Events;
using Shared.Inbox.Models;
using Shared.Inbox.Settings;
using Shared.Inbox.Storage;
using Shared.Messaging.Connection;

namespace Shared.Inbox.Services;

public class InboxIntegrationEventConsumer<TEvent>(
    string moduleName,
    string exchangeName,
    ILogger<InboxIntegrationEventConsumer<TEvent>> logger,
    IMessageBusConnectionFactory busFactory,
    IInboxStorage inboxStorage,
    InboxSettings settings)
    : BackgroundService
    where TEvent : IIntegrationEvent
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var connection = await busFactory.CreateConnectionAsync(stoppingToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        var queueName = await DeclareQueueAsync(channel, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, args) =>
        {
            var integrationEvent = DeserializeEvent(args);

            if (await inboxStorage.IsAlreadyProcessedAsync(integrationEvent.Id, stoppingToken))
                return;

            var headers = ExtractHeaders(args);

            using (logger.BeginScope(headers))
            {
                var inboxMessage = InboxMessage.Create(
                    integrationEvent.Id,
                    integrationEvent,
                    integrationEvent.OccurredOn,
                    headers
                    );

                await inboxStorage.AddAsync(inboxMessage, stoppingToken);

                await channel.BasicAckAsync(args.DeliveryTag, false);

                logger.LogInformation("Consumed message '{MessageType}' with id '{Id}' in '{Module}'",
                    inboxMessage.GetTypeName(), integrationEvent.Id, moduleName);
            }
        };

        await channel.BasicConsumeAsync(queueName, false, consumer, cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(settings.IntervalInSeconds), stoppingToken);
        }
    }

    private async Task<string> DeclareQueueAsync(IChannel channel, CancellationToken stoppingToken)
    {
        var queueName = $"{moduleName.ToLowerInvariant()}-{exchangeName}";

        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(queueName, exclusive: false, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(queueName, exchangeName, string.Empty, cancellationToken: stoppingToken);

        return queueName;
    }

    private static Dictionary<string, string> ExtractHeaders(BasicDeliverEventArgs args)
    {
        return args.BasicProperties.Headers?
            .ToDictionary(
                header => header.Key,
                header => Encoding.UTF8.GetString((byte[])header.Value!))
            ?? [];
    }

    private static TEvent DeserializeEvent(BasicDeliverEventArgs args)
    {
        var body = args.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var integrationEvent = JsonSerializer.Deserialize<TEvent>(message);

        return integrationEvent ?? throw new InvalidOperationException($"Can't deserialize the message '{message}'");
    }
}

