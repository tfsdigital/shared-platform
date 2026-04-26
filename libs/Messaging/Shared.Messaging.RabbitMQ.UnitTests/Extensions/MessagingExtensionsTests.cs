using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Shared.Messaging.Abstractions.Interfaces;
using Shared.Messaging.Abstractions.Extensions;
using Shared.Messaging.Abstractions.Models;
using Shared.Messaging.RabbitMQ.Connection;
using Shared.Messaging.RabbitMQ.Consumers;
using Shared.Messaging.RabbitMQ.Extensions;
using Shared.Messaging.RabbitMQ.Options;
using Shared.Messaging.RabbitMQ.Topology;

using NSubstitute;

using RabbitMQ.Client;

namespace Shared.Messaging.RabbitMQ.UnitTests.Extensions;

public class RabbitMqMessagingExtensionsTests
{
    [Fact]
    public void UseRabbitMq_ShouldRegisterMessageBusAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMessaging().UseRabbitMq(o => o.ConnectionString = "amqp://localhost");

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMessageBus));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void UseRabbitMq_ShouldRegisterConnectionFactoryAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMessaging().UseRabbitMq(o => o.ConnectionString = "amqp://localhost");

        // Assert
        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IRabbitMqConnectionFactory));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void UseRabbitMq_ShouldRegisterPersistentConnectionAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMessaging().UseRabbitMq(o => o.ConnectionString = "amqp://localhost");

        // Assert
        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IPersistentRabbitMqConnection));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void UseRabbitMq_ShouldReturnMessagingBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddMessaging();

        // Act
        var result = builder.UseRabbitMq(o => o.ConnectionString = "amqp://localhost");

        // Assert
        Assert.Same(builder, result);
    }

    [Fact]
    public void AddPublishOptions_ShouldRegisterPublishTopologyEntry()
    {
        var services = new ServiceCollection();
        var builder = services.AddMessaging();

        builder.AddPublishOptions<TestMessage>(o =>
        {
            o.Destination = "events";
            o.RoutingKey = "test";
        });

        var entry = Assert.Single(services, d => d.ServiceType == typeof(PublishTopologyEntry));
        var topologyEntry = Assert.IsType<PublishTopologyEntry>(entry.ImplementationInstance);
        Assert.Equal(typeof(TestMessage), topologyEntry.MessageType);
    }

    [Fact]
    public void AddConsumer_ShouldRegisterConsumerOptionsConsumerAndHostedService()
    {
        var services = new ServiceCollection();
        var builder = services.AddMessaging();

        builder.AddConsumer<TestConsumer, TestMessage>(o =>
        {
            o.Exchange     = "events";
            o.Queue        = "queue";
            o.ConsumerName = "consumer";
        });

        Assert.Contains(services, d => d.ServiceType == typeof(RabbitMqConsumerOptions));
        Assert.Contains(services, d => d.ServiceType == typeof(TestConsumer)
            && d.Lifetime == ServiceLifetime.Scoped);
        Assert.Contains(services, d => d.ServiceType == typeof(IHostedService));
    }

    [Fact]
    public async Task ApplyRabbitMqTopologyAsync_DeclaresPublishAndConsumerTopology()
    {
        var services = new ServiceCollection();
        var connection = Substitute.For<IPersistentRabbitMqConnection>();
        var channel = Substitute.For<IChannel>();
        var publishOptions = new RabbitMqPublishOptions
        {
            Destination = "events",
            RoutingKey = "published",
            ExchangeType = RabbitMqExchangeType.Topic,
            Durable = true
        };
        var consumerOptions = new RabbitMqConsumerOptions
        {
            Exchange = "events",
            Queue = "queue",
            RoutingKey = "consumed",
            ExchangeType = RabbitMqExchangeType.Topic,
            ConsumerName = "consumer",
            Durable = true
        };
        connection.CreateChannelAsync(
                Arg.Any<CreateChannelOptions?>(),
                Arg.Any<CancellationToken>())
            .Returns(channel);
        services.AddSingleton(connection);
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        services.AddSingleton(new PublishTopologyEntry(typeof(TestMessage), publishOptions));
        services.AddSingleton(consumerOptions);
        using var provider = services.BuildServiceProvider();
        using var host = new FakeHost(provider);

        await host.ApplyRabbitMqTopologyAsync(CancellationToken.None);

        await connection.Received(1).EnsureConnectedAsync(Arg.Any<CancellationToken>());
        await channel.Received().ExchangeDeclareAsync(
            "events",
            ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            Arg.Any<IDictionary<string, object?>>(),
            noWait: false,
            Arg.Any<CancellationToken>());
        await channel.Received(1).QueueDeclareAsync(
            "queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            Arg.Any<IDictionary<string, object?>>(),
            noWait: false,
            Arg.Any<CancellationToken>());
    }

    private sealed record TestMessage;

    private sealed class TestConsumer : IMessageConsumer<TestMessage>
    {
        public Task<ConsumerResult> ConsumeAsync(
            TestMessage message,
            IMessageContext context,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(ConsumerResult.Ack());
    }

    private sealed class FakeHost(IServiceProvider services) : IHost
    {
        public IServiceProvider Services { get; } = services;

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
