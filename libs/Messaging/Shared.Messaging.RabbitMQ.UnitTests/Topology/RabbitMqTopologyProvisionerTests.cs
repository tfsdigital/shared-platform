using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using RabbitMQ.Client;

using Shared.Messaging.RabbitMQ.Options;
using Shared.Messaging.RabbitMQ.Topology;

namespace Shared.Messaging.RabbitMQ.UnitTests.Topology;

public class RabbitMqTopologyProvisionerTests
{
    [Theory]
    [InlineData(RabbitMqExchangeType.Direct, ExchangeType.Direct)]
    [InlineData(RabbitMqExchangeType.Topic, ExchangeType.Topic)]
    [InlineData(RabbitMqExchangeType.Headers, ExchangeType.Headers)]
    [InlineData(RabbitMqExchangeType.Fanout, ExchangeType.Fanout)]
    public void ToExchangeTypeString_MapsExchangeTypes(
        RabbitMqExchangeType exchangeType,
        string expected)
    {
        var actual = exchangeType.ToExchangeTypeString();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task DeclareExchangeAsync_DeclaresExchange()
    {
        var channel = Substitute.For<IChannel>();

        await RabbitMqTopologyProvisioner.DeclareExchangeAsync(
            channel,
            exchange: "orders",
            type: ExchangeType.Topic,
            durable: true,
            NullLogger.Instance,
            CancellationToken.None);

        await channel.Received(1).ExchangeDeclareAsync(
            "orders",
            ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            Arg.Any<IDictionary<string, object?>>(),
            noWait: false,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeclareConsumerTopologyAsync_WithoutDeadLetter_DeclaresExchangeQueueAndBinding()
    {
        var channel = Substitute.For<IChannel>();
        var options = new RabbitMqConsumerOptions
        {
            Exchange              = "events",
            ExchangeType          = RabbitMqExchangeType.Topic,
            Queue                 = "orders.created",
            RoutingKey            = "orders.created",
            Durable               = true,
            Exclusive             = false,
            AutoDelete            = false,
            EnableDeadLetterQueue = false
        };

        await RabbitMqTopologyProvisioner.DeclareConsumerTopologyAsync(
            channel,
            options,
            NullLogger.Instance,
            CancellationToken.None);

        await channel.Received(1).ExchangeDeclareAsync(
            options.Exchange,
            ExchangeType.Topic,
            options.Durable,
            autoDelete: false,
            Arg.Any<IDictionary<string, object?>>(),
            noWait: false,
            Arg.Any<CancellationToken>());
        await channel.Received(1).QueueDeclareAsync(
            options.Queue,
            options.Durable,
            options.Exclusive,
            options.AutoDelete,
            Arg.Is<IDictionary<string, object?>>(args => args.Count == 0),
            noWait: false,
            Arg.Any<CancellationToken>());
        await channel.Received(1).QueueBindAsync(
            options.Queue,
            options.Exchange,
            options.RoutingKey,
            Arg.Any<IDictionary<string, object?>>(),
            noWait: false,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeclareConsumerTopologyAsync_WithDeadLetter_DeclaresDeadLetterTopology()
    {
        var channel = Substitute.For<IChannel>();
        var options = new RabbitMqConsumerOptions
        {
            Exchange              = "events",
            Queue                 = "orders.created",
            RoutingKey            = "orders.created",
            Durable               = true,
            EnableDeadLetterQueue = true,
            DeadLetterExchange    = "events.dlx",
            DeadLetterRoutingKey  = "orders.created.dead"
        };

        await RabbitMqTopologyProvisioner.DeclareConsumerTopologyAsync(
            channel,
            options,
            NullLogger.Instance,
            CancellationToken.None);

        await channel.Received(1).QueueDeclareAsync(
            options.Queue,
            options.Durable,
            options.Exclusive,
            options.AutoDelete,
            Arg.Is<IDictionary<string, object?>>(args =>
                Equals(args["x-dead-letter-exchange"], options.ResolvedDeadLetterExchange)
                && Equals(args["x-dead-letter-routing-key"], options.ResolvedDeadLetterRoutingKey)),
            noWait: false,
            Arg.Any<CancellationToken>());
        await channel.Received(1).ExchangeDeclareAsync(
            options.ResolvedDeadLetterExchange,
            ExchangeType.Direct,
            options.Durable,
            autoDelete: false,
            Arg.Any<IDictionary<string, object?>>(),
            noWait: false,
            Arg.Any<CancellationToken>());
        await channel.Received(1).QueueDeclareAsync(
            options.ResolvedDeadLetterQueue,
            options.Durable,
            exclusive: false,
            autoDelete: false,
            Arg.Any<IDictionary<string, object?>>(),
            noWait: false,
            Arg.Any<CancellationToken>());
        await channel.Received(1).QueueBindAsync(
            options.ResolvedDeadLetterQueue,
            options.ResolvedDeadLetterExchange,
            options.ResolvedDeadLetterRoutingKey,
            Arg.Any<IDictionary<string, object?>>(),
            noWait: false,
            Arg.Any<CancellationToken>());
    }
}
