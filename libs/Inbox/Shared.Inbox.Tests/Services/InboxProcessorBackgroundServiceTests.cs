using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Polly;
using Shared.Core.Events;
using Shared.Correlation.Context;
using Shared.Inbox.Models;
using Shared.Inbox.Services;
using Shared.Inbox.Settings;
using Shared.Inbox.Storage;
using Shared.Publishing;

namespace Shared.Inbox.Tests.Services;

public class InboxProcessorBackgroundServiceTests
{
    private record TestIntegrationEvent(string Name) : IIntegrationEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    private readonly ICorrelationContext _correlationContext;
    private readonly IEventPublisher _publisher;
    private readonly ILogger<InboxProcessorBackgroundService> _logger;
    private readonly IInboxStorage _inboxStorage;
    private readonly InboxSettings _settings;

    public InboxProcessorBackgroundServiceTests()
    {
        _correlationContext = Substitute.For<ICorrelationContext>();
        _publisher = Substitute.For<IEventPublisher>();
        _logger = Substitute.For<ILogger<InboxProcessorBackgroundService>>();
        _inboxStorage = Substitute.For<IInboxStorage>();
        _settings = new InboxSettings { IntervalInSeconds = 0, MessagesBatchSize = 10 };
    }

    private InboxProcessorBackgroundService CreateService() =>
        new(
            "test-module",
            _correlationContext,
            _publisher,
            _logger,
            _inboxStorage,
            ResiliencePipeline.Empty,
            _settings
        );

    [Fact]
    public async Task ExecuteAsync_WhenNoMessages_ShouldNotCallPublisher()
    {
        // Arrange
        _inboxStorage
            .GetUnprocessedMessagesAsync(Arg.Any<CancellationToken>())
            .Returns(Array.Empty<InboxMessage>());

        var cts = new CancellationTokenSource();
        var service = CreateService();

        // Act
        cts.CancelAfter(50);
        await service.StartAsync(cts.Token);
        await Task.Delay(100);
        await service.StopAsync(CancellationToken.None);

        // Assert
        await _publisher
            .DidNotReceive()
            .Publish(Arg.Any<IIntegrationEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenMessageExists_ShouldPublishMessageAndMarkAsProcessed()
    {
        // Arrange
        var testEvent = new TestIntegrationEvent("test");
        var message = InboxMessage.Create(Guid.NewGuid(), testEvent, DateTime.UtcNow);

        var callCount = 0;
        _inboxStorage
            .GetUnprocessedMessagesAsync(Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                callCount++;
                if (callCount == 1)
                    return Task.FromResult<IReadOnlyList<InboxMessage>>(new[] { message });
                return Task.FromResult<IReadOnlyList<InboxMessage>>(Array.Empty<InboxMessage>());
            });

        var cts = new CancellationTokenSource();
        var service = CreateService();

        // Act
        await service.StartAsync(cts.Token);
        await Task.Delay(150);
        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        // Assert
        await _publisher
            .Received(1)
            .Publish(Arg.Any<IIntegrationEvent>(), Arg.Any<CancellationToken>());

        Assert.NotNull(message.ProcessedOn);
        Assert.Null(message.Error);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPublishingFails_ShouldMarkMessageWithError()
    {
        // Arrange
        var testEvent = new TestIntegrationEvent("fail");
        var message = InboxMessage.Create(Guid.NewGuid(), testEvent, DateTime.UtcNow);

        var callCount = 0;
        _inboxStorage
            .GetUnprocessedMessagesAsync(Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                callCount++;
                if (callCount == 1)
                    return Task.FromResult<IReadOnlyList<InboxMessage>>(new[] { message });
                return Task.FromResult<IReadOnlyList<InboxMessage>>(Array.Empty<InboxMessage>());
            });

        _publisher
            .Publish(Arg.Any<IIntegrationEvent>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("publish failed"));

        var cts = new CancellationTokenSource();
        var service = CreateService();

        // Act
        await service.StartAsync(cts.Token);
        await Task.Delay(150);
        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(message.ProcessedOn);
        Assert.NotNull(message.Error);
        Assert.Equal("publish failed", message.Error);
        Assert.NotNull(message.ErrorHandledOn);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancellationTokenPreCancelled_ShouldExitWithoutProcessing()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var service = CreateService();

        // Act & Assert - should complete without entering the while loop
        await service.StartAsync(cts.Token);
        await service.StopAsync(CancellationToken.None);

        await _inboxStorage
            .DidNotReceive()
            .GetUnprocessedMessagesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenMessageHasCorrelationHeader_ShouldSetCorrelationContext()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var headers = new Dictionary<string, string>
        {
            { "correlation_id", correlationId },
        };

        var testEvent = new TestIntegrationEvent("correlation-test");
        var message = InboxMessage.Create(Guid.NewGuid(), testEvent, DateTime.UtcNow, headers);

        var callCount = 0;
        _inboxStorage
            .GetUnprocessedMessagesAsync(Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                callCount++;
                if (callCount == 1)
                    return Task.FromResult<IReadOnlyList<InboxMessage>>(new[] { message });
                return Task.FromResult<IReadOnlyList<InboxMessage>>(Array.Empty<InboxMessage>());
            });

        var cts = new CancellationTokenSource();
        var service = CreateService();

        // Act
        await service.StartAsync(cts.Token);
        await Task.Delay(150);
        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        // Assert
        _correlationContext.Received(1).SetCorrelationId(correlationId);
    }
}
