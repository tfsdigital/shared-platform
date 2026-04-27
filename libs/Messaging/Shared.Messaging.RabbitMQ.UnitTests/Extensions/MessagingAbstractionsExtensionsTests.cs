using Microsoft.Extensions.DependencyInjection;

using Shared.Messaging.Abstractions.Extensions;
using Shared.Messaging.Abstractions.Models;

namespace Shared.Messaging.RabbitMQ.UnitTests.Extensions;

public class MessagingAbstractionsExtensionsTests
{
    [Fact]
    public void AddMessaging_ReturnsBuilderWithServices()
    {
        var services = new ServiceCollection();

        var builder = services.AddMessaging();

        Assert.Same(services, builder.Services);
    }

    [Fact]
    public void AddPublishOptions_RegistersPublishTopologyEntryAndReturnsBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddMessaging();

        var result = Shared.Messaging.Abstractions.Extensions.MessagingExtensions
            .AddPublishOptions<TestMessage>(builder, options => options.Destination = "events");

        Assert.Same(builder, result);
        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(PublishTopologyEntry));
        var entry = Assert.IsType<PublishTopologyEntry>(descriptor.ImplementationInstance);
        Assert.Equal(typeof(TestMessage), entry.MessageType);
        Assert.Equal("events", entry.Options.Destination);
    }

    [Fact]
    public void PublishOptionsValidate_WhenDestinationIsMissing_Throws()
    {
        var options = new PublishOptions();

        Assert.Throws<ArgumentException>(() => options.Validate());
    }

    [Fact]
    public void ConsumerOptionsValidate_DoesNotThrow()
    {
        var options = new ConsumerOptions();

        options.Validate();
    }

    private sealed record TestMessage;
}
