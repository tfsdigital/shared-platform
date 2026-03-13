using Microsoft.Extensions.Logging;
using NSubstitute;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Core.Events;
using Shared.Inbox.Models;
using Shared.Inbox.Services;
using Shared.Inbox.Settings;
using Shared.Inbox.Storage;
using Shared.Messaging.Connection;
using System.Text;
using System.Text.Json;

namespace Shared.Inbox.Tests.Services;

public record TestConsumerEvent : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public string Data { get; init; } = string.Empty;
}

public class InboxIntegrationEventConsumerTests
{
    private readonly IMessageBusConnectionFactory _busFactory;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly IInboxStorage _inboxStorage;
    private readonly ILogger<InboxIntegrationEventConsumer<TestConsumerEvent>> _logger;

    public InboxIntegrationEventConsumerTests()
    {
        _busFactory = Substitute.For<IMessageBusConnectionFactory>();
        _connection = Substitute.For<IConnection>();
        _channel = Substitute.For<IChannel>();
        _inboxStorage = Substitute.For<IInboxStorage>();
        _logger = Substitute.For<ILogger<InboxIntegrationEventConsumer<TestConsumerEvent>>>();

        _busFactory.CreateConnectionAsync(Arg.Any<CancellationToken>()).Returns(_connection);
        _connection
            .CreateChannelAsync(cancellationToken: Arg.Any<CancellationToken>())
            .Returns(_channel);
    }

    private TestableInboxIntegrationEventConsumer CreateConsumer(
        InboxSettings? settings = null,
        string moduleName = "Test-Module",
        string exchangeName = "test-exchange"
    ) =>
        new(
            moduleName,
            exchangeName,
            _logger,
            _busFactory,
            _inboxStorage,
            settings
                ?? new InboxSettings
                {
                    ConnectionString = "Host=localhost;Database=test",
                    IntervalInSeconds = 1,
                    MessagesBatchSize = 10,
                }
        );

    private TaskCompletionSource<AsyncEventingBasicConsumer> SetupConsumerCapture(
        Action<AsyncEventingBasicConsumer>? onCapture = null
    )
    {
        var consumerRegistered =
            new TaskCompletionSource<AsyncEventingBasicConsumer>(TaskCreationOptions.RunContinuationsAsynchronously);

        _channel
            .When(c => c.BasicConsumeAsync(
                Arg.Any<string>(),
                Arg.Any<bool>(),
                Arg.Any<string>(),
                Arg.Any<bool>(),
                Arg.Any<bool>(),
                Arg.Any<IDictionary<string, object?>?>(),
                Arg.Any<IAsyncBasicConsumer>(),
                Arg.Any<CancellationToken>()
            ))
            .Do(ci =>
            {
                var consumer = Assert.IsType<AsyncEventingBasicConsumer>(ci.ArgAt<IAsyncBasicConsumer>(6));
                onCapture?.Invoke(consumer);
                consumerRegistered.TrySetResult(consumer);
            });

        return consumerRegistered;
    }

    private static byte[] Serialize(TestConsumerEvent integrationEvent) =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(integrationEvent));

    private static IReadOnlyBasicProperties CreateProperties(
        IDictionary<string, object?>? headers = null
    )
    {
        var properties = Substitute.For<IReadOnlyBasicProperties>();
        properties.Headers.Returns(headers);
        return properties;
    }

    [Fact]
    public async Task ExecuteAsync_ShouldDeclareQueueUsingLowercaseModuleName()
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var consumerRegistered = SetupConsumerCapture(_ => cts.Cancel());
        var service = CreateConsumer();

        // Act
        await service.RunAsync(cts.Token);
        await consumerRegistered.Task;

        // Assert
        await _channel
            .Received(1)
            .ExchangeDeclareAsync(
                "test-exchange",
                ExchangeType.Fanout,
                cancellationToken: Arg.Any<CancellationToken>()
            );

        await _channel
            .Received(1)
            .QueueDeclareAsync(
                "test-module-test-exchange",
                exclusive: false,
                cancellationToken: Arg.Any<CancellationToken>()
            );

        await _channel
            .Received(1)
            .QueueBindAsync(
                "test-module-test-exchange",
                "test-exchange",
                string.Empty,
                cancellationToken: Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_ShouldStartConsumingFromDeclaredQueue()
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var consumerRegistered = SetupConsumerCapture(_ => cts.Cancel());
        var service = CreateConsumer();

        // Act
        await service.RunAsync(cts.Token);
        var consumer = await consumerRegistered.Task;

        // Assert
        await _channel
            .Received(1)
            .BasicConsumeAsync(
                "test-module-test-exchange",
                false,
                string.Empty,
                false,
                false,
                Arg.Is<IDictionary<string, object?>?>(value => value == null),
                consumer,
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenMessageReceived_ShouldPersistMessageAndAcknowledge()
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var consumerRegistered = SetupConsumerCapture();
        var service = CreateConsumer();
        var runTask = service.RunAsync(cts.Token);

        var consumer = await consumerRegistered.Task;
        var integrationEvent = new TestConsumerEvent { Data = "test-data" };
        InboxMessage? persistedMessage = null;

        _inboxStorage
            .IsAlreadyProcessedAsync(integrationEvent.Id, Arg.Any<CancellationToken>())
            .Returns(false);
        _inboxStorage
            .When(storage => storage.AddAsync(Arg.Any<InboxMessage>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => persistedMessage = callInfo.Arg<InboxMessage>());

        // Act
        await consumer.HandleBasicDeliverAsync(
            "consumer-tag",
            7,
            false,
            "test-exchange",
            "routing-key",
            CreateProperties(),
            Serialize(integrationEvent)
        );

        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runTask);

        // Assert
        await _inboxStorage.Received(1).AddAsync(Arg.Any<InboxMessage>(), Arg.Any<CancellationToken>());
        Assert.NotNull(persistedMessage);
        Assert.Equal(integrationEvent.Id, persistedMessage.Id);
        Assert.Equal(nameof(TestConsumerEvent), persistedMessage.GetTypeName());
        Assert.Equal("{}", persistedMessage.Headers);

        await _channel.Received(1).BasicAckAsync(7, false, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenMessageAlreadyProcessed_ShouldSkipPersistenceAndAcknowledge()
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var consumerRegistered = SetupConsumerCapture();
        var service = CreateConsumer();
        var runTask = service.RunAsync(cts.Token);

        var consumer = await consumerRegistered.Task;
        var integrationEvent = new TestConsumerEvent { Data = "already-processed" };

        _inboxStorage
            .IsAlreadyProcessedAsync(integrationEvent.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await consumer.HandleBasicDeliverAsync(
            "consumer-tag",
            9,
            false,
            "test-exchange",
            "routing-key",
            CreateProperties(),
            Serialize(integrationEvent)
        );

        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runTask);

        // Assert
        await _inboxStorage
            .DidNotReceive()
            .AddAsync(Arg.Any<InboxMessage>(), Arg.Any<CancellationToken>());

        await _channel
            .DidNotReceive()
            .BasicAckAsync(Arg.Any<ulong>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenHeadersExist_ShouldPersistExtractedHeaders()
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var consumerRegistered = SetupConsumerCapture();
        var service = CreateConsumer();
        var runTask = service.RunAsync(cts.Token);

        var consumer = await consumerRegistered.Task;
        var integrationEvent = new TestConsumerEvent { Data = "with-headers" };
        var correlationId = Guid.NewGuid().ToString();
        var tenant = "tenant-01";
        InboxMessage? persistedMessage = null;

        _inboxStorage
            .IsAlreadyProcessedAsync(integrationEvent.Id, Arg.Any<CancellationToken>())
            .Returns(false);
        _inboxStorage
            .When(storage => storage.AddAsync(Arg.Any<InboxMessage>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => persistedMessage = callInfo.Arg<InboxMessage>());

        var headers = new Dictionary<string, object?>
        {
            ["correlation_id"] = Encoding.UTF8.GetBytes(correlationId),
            ["tenant"] = Encoding.UTF8.GetBytes(tenant),
        };

        // Act
        await consumer.HandleBasicDeliverAsync(
            "consumer-tag",
            11,
            false,
            "test-exchange",
            "routing-key",
            CreateProperties(headers),
            Serialize(integrationEvent)
        );

        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runTask);

        // Assert
        await _inboxStorage.Received(1).AddAsync(Arg.Any<InboxMessage>(), Arg.Any<CancellationToken>());

        var persistedHeaders = persistedMessage?.GetHeaders();
        Assert.NotNull(persistedHeaders);
        Assert.Equal(correlationId, persistedHeaders["correlation_id"]);
        Assert.Equal(tenant, persistedHeaders["tenant"]);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPayloadIsInvalid_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var consumerRegistered = SetupConsumerCapture();
        var service = CreateConsumer();
        var runTask = service.RunAsync(cts.Token);

        var consumer = await consumerRegistered.Task;
        var invalidPayload = Encoding.UTF8.GetBytes("{\"data\":");

        // Act
        var exception = await Assert.ThrowsAnyAsync<Exception>(() =>
            consumer.HandleBasicDeliverAsync(
                "consumer-tag",
                13,
                false,
                "test-exchange",
                "routing-key",
                CreateProperties(),
                invalidPayload
            )
        );

        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runTask);

        // Assert
        Assert.True(
            exception is JsonException or InvalidOperationException,
            $"Unexpected exception type: {exception.GetType().Name}"
        );

        await _inboxStorage
            .DidNotReceive()
            .AddAsync(Arg.Any<InboxMessage>(), Arg.Any<CancellationToken>());
    }

    private sealed class TestableInboxIntegrationEventConsumer(
        string moduleName,
        string exchangeName,
        ILogger<InboxIntegrationEventConsumer<TestConsumerEvent>> logger,
        IMessageBusConnectionFactory busFactory,
        IInboxStorage inboxStorage,
        InboxSettings settings
    ) : InboxIntegrationEventConsumer<TestConsumerEvent>(
        moduleName,
        exchangeName,
        logger,
        busFactory,
        inboxStorage,
        settings
    )
    {
        public Task RunAsync(CancellationToken cancellationToken) => ExecuteAsync(cancellationToken);
    }
}
