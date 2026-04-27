using Shared.Messaging.Abstractions.Models;
using Shared.Messaging.RabbitMQ.Topology;

namespace Shared.Messaging.RabbitMQ.UnitTests.Topology;

public class PublishTopologyRegistryTests
{
    [Fact]
    public void GetOptions_WhenMessageTypeRegistered_ReturnsOptions()
    {
        var options = new PublishOptions { Destination = "events" };
        var registry = new PublishTopologyRegistry(
        [
            new PublishTopologyEntry(typeof(TestMessage), options)
        ]);

        var actual = registry.GetOptions(typeof(TestMessage));

        Assert.Same(options, actual);
    }

    [Fact]
    public void GetOptions_WhenMessageTypeNotRegistered_ReturnsNull()
    {
        var registry = new PublishTopologyRegistry([]);

        var actual = registry.GetOptions(typeof(TestMessage));

        Assert.Null(actual);
    }

    private sealed record TestMessage;
}
