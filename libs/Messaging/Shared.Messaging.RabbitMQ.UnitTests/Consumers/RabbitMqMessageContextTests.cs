using System.Text;

using NSubstitute;

using RabbitMQ.Client;

using Shared.Messaging.RabbitMQ.Consumers;

namespace Shared.Messaging.RabbitMQ.UnitTests.Consumers;

public class RabbitMqMessageContextTests
{
    [Fact]
    public void Constructor_DecodesHeadersAndMessageMetadata()
    {
        var headers = new Dictionary<string, object?>
        {
            ["bytes"] = Encoding.UTF8.GetBytes("from-bytes"),
            ["number"] = 123,
            ["null"] = null
        };

        var context = new RabbitMqMessageContext(
            Substitute.For<IChannel>(),
            deliveryTag: 42,
            headers,
            messageId: "message-1",
            redelivered: true);

        Assert.Equal("from-bytes", context.Headers["bytes"]);
        Assert.Equal("123", context.Headers["number"]);
        Assert.Equal(string.Empty, context.Headers["null"]);
        Assert.Equal("message-1", context.MessageId);
        Assert.True(context.Redelivered);
    }

    [Fact]
    public async Task AckAsync_CallsChannelAck()
    {
        var channel = Substitute.For<IChannel>();
        var context = new RabbitMqMessageContext(channel, 42, null, null, false);

        await context.AckAsync(multiple: true, CancellationToken.None);

        await channel.Received(1).BasicAckAsync(42, multiple: true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task NackAsync_CallsChannelNack()
    {
        var channel = Substitute.For<IChannel>();
        var context = new RabbitMqMessageContext(channel, 42, null, null, false);

        await context.NackAsync(multiple: true, requeue: false, CancellationToken.None);

        await channel.Received(1).BasicNackAsync(42, multiple: true, requeue: false, Arg.Any<CancellationToken>());
    }
}
