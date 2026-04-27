using System.Diagnostics.Metrics;

using NSubstitute;

using Shared.Inbox.Abstractions.Metrics;

namespace Shared.Inbox.Abstractions.UnitTests.Metrics;

public class InboxMetricsTests
{
    [Fact]
    public void RecordMethods_RecordMeasurements()
    {
        using var meter = new Meter(InboxInstrumentation.MeterName);
        var meterFactory = Substitute.For<IMeterFactory>();
        meterFactory.Create(Arg.Any<MeterOptions>()).Returns(meter);
        using var metrics = new InboxMetrics(meterFactory);

        metrics.RecordRegistered();
        metrics.RecordDuplicate();
        metrics.RecordHandlerDuration(12.5);

        meterFactory.Received(1).Create(
            Arg.Is<MeterOptions>(options => options.Name == InboxInstrumentation.MeterName));
    }
}
