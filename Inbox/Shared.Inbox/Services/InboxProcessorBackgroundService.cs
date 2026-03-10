using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Shared.Correlation.Context;
using Shared.Inbox.Models;
using Shared.Inbox.Settings;
using Shared.Inbox.Storage;
using Shared.Publishing;

namespace Shared.Inbox.Services;

public class InboxProcessorBackgroundService(
    string moduleName,
    ICorrelationContext correlationContext,
    IEventPublisher publisher,
    ILogger<InboxProcessorBackgroundService> logger,
    IInboxStorage inboxStorage,
    ResiliencePipeline resiliencePipeline,
    InboxSettings settings)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var inboxMessages = await inboxStorage.GetUnprocessedMessagesAsync(stoppingToken);

            foreach (var inboxMessage in inboxMessages)
            {
                var headers = inboxMessage.GetHeaders();

                SetCorrelationId(correlationContext, headers);

                using (logger.BeginScope(headers ?? []))
                {
                    await ProcessMessage(inboxMessage, stoppingToken);

                    await inboxStorage.UpdateMessageAsync(inboxMessage, stoppingToken);
                }
            }

            await inboxStorage.CommitAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(settings.IntervalInSeconds), stoppingToken);
        }
    }

    private async Task ProcessMessage(InboxMessage inboxMessage, CancellationToken cancellationToken)
    {
        try
        {
            await resiliencePipeline.ExecuteAsync(async internalCancelationToken =>
            {
                var integrationEvent = inboxMessage.GetContent();

                await publisher.Publish(integrationEvent, internalCancelationToken);

                inboxMessage.MarkAsProcessedWithSuccess();

                logger.LogInformation("Processed message '{MessageType}' with id '{Id}' in '{Module}'",
                    inboxMessage.GetTypeName(), inboxMessage.Id, moduleName);

            }, cancellationToken);
        }
        catch (Exception exception)
        {
            inboxMessage.MarkAsProcessedWithError(exception.Message);

            logger.LogError(exception, "Failed to process message '{MessageType}' with id '{Id}' in '{Module}'",
                inboxMessage.GetTypeName(), inboxMessage.Id, moduleName);
        }
    }

    private static void SetCorrelationId(
        ICorrelationContext correlationAccessor, Dictionary<string, string>? headers)
    {
        if (headers is null)
            return;

        var correlationId = headers["correlation_id"].ToString();

        if (correlationId is null)
            return;

        correlationAccessor.SetCorrelationId(correlationId);
    }
}