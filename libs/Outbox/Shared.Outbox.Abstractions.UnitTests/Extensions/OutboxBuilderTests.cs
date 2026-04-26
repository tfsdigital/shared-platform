using System.Diagnostics.Metrics;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using Polly;

using Shared.Outbox.Abstractions.Extensions;
using Shared.Outbox.Abstractions.Metrics;

namespace Shared.Outbox.Abstractions.UnitTests.Extensions;

public class OutboxBuilderTests
{
    [Fact]
    public void WithSettings_WhenOptionsAreValid_ReturnsBuilder()
    {
        var builder = new OutboxBuilder(new ServiceCollection(), moduleName: null);

        var result = builder.WithSettings(options => options.BatchSize = 25);

        Assert.Same(builder, result);
    }

    [Fact]
    public void WithResilience_ReplacesPipelineAndReturnsBuilder()
    {
        var builder = new OutboxBuilder(new ServiceCollection(), moduleName: null);
        var pipeline = new ResiliencePipelineBuilder().Build();

        var result = builder.WithResilience(pipeline);

        Assert.Same(builder, result);
    }

    [Fact]
    public void WithMetrics_WhenNotKeyed_RegistersMetrics()
    {
        var services = new ServiceCollection();
        using var meter = new Meter(OutboxMetrics.MeterName);
        var meterFactory = Substitute.For<IMeterFactory>();
        meterFactory.Create(Arg.Any<MeterOptions>()).Returns(meter);
        services.AddSingleton(meterFactory);
        var builder = new OutboxBuilder(services, moduleName: "orders");

        var result = builder.WithMetrics(options =>
            options.Tags = new Dictionary<string, string> { ["tenant"] = "default" });

        Assert.Same(builder, result);
        using var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetRequiredService<IOutboxMetrics>());
    }

    [Fact]
    public void WithMetrics_WhenKeyed_RegistersKeyedMetrics()
    {
        var services = new ServiceCollection();
        using var meter = new Meter(OutboxMetrics.MeterName);
        var meterFactory = Substitute.For<IMeterFactory>();
        meterFactory.Create(Arg.Any<MeterOptions>()).Returns(meter);
        services.AddSingleton(meterFactory);
        var builder = new OutboxBuilder(services, moduleName: "orders", isKeyed: true);

        builder.WithMetrics();

        using var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetRequiredKeyedService<IOutboxMetrics>("orders"));
    }
}
