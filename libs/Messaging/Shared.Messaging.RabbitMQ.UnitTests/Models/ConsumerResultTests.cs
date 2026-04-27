using Shared.Messaging.Abstractions.Models;

namespace Shared.Messaging.RabbitMQ.UnitTests.Models;

public class ConsumerResultTests
{
    [Fact]
    public void Ack_CreatesAckResult()
    {
        var result = ConsumerResult.Ack();

        Assert.Equal(ConsumerResultStatus.Ack, result.Status);
        Assert.True(result.IsAck);
        Assert.False(result.IsNack);
        Assert.False(result.Requeue);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Nack_WithDefaultArguments_CreatesRequeueableNack()
    {
        var result = ConsumerResult.Nack();

        Assert.Equal(ConsumerResultStatus.Nack, result.Status);
        Assert.False(result.IsAck);
        Assert.True(result.IsNack);
        Assert.True(result.Requeue);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Nack_WithExplicitArguments_CapturesRequeueAndError()
    {
        var result = ConsumerResult.Nack(requeue: false, error: "invalid message");

        Assert.Equal(ConsumerResultStatus.Nack, result.Status);
        Assert.False(result.Requeue);
        Assert.Equal("invalid message", result.Error);
    }
}
