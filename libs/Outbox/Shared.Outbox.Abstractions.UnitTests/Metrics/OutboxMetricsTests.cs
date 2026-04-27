using System.Diagnostics.Metrics;

using NSubstitute;

using Shared.Outbox.Abstractions.Metrics;

namespace Shared.Outbox.Abstractions.UnitTests.Metrics;

public class OutboxMetricsTests
{
    [Fact]
    public void RecordMethods_RecordMeasurementsWithTags()
    {
        using var meter = new Meter(OutboxMetrics.MeterName);
        var meterFactory = Substitute.For<IMeterFactory>();
        meterFactory.Create(Arg.Any<MeterOptions>()).Returns(meter);
        var globalTags = new Dictionary<string, string> { ["service"] = "orders" };
        var additionalTags = new Dictionary<string, string> { ["tenant"] = "default" };
        using var metrics = new OutboxMetrics(meterFactory, globalTags);

        metrics.RecordPublished(additionalTags);
        metrics.RecordFailed(additionalTags);
        metrics.RecordProcessed(additionalTags);
        metrics.RecordFetchDuration(1, additionalTags);
        metrics.RecordUpdateDuration(2, additionalTags);
        metrics.RecordPublishDuration(3, additionalTags);
        metrics.RecordCycleDuration(4, additionalTags);
        metrics.RecordBatchSize(5, additionalTags);

        meterFactory.Received(1).Create(
            Arg.Is<MeterOptions>(options => options.Name == OutboxMetrics.MeterName));
    }
}
