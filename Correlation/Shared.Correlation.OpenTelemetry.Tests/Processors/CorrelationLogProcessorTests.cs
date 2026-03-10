using NSubstitute;
using OpenTelemetry.Logs;
using Shared.Correlation.Context;
using Shared.Correlation.OpenTelemetry.Processors;

namespace Shared.Correlation.OpenTelemetry.Tests.Processors;

public class CorrelationLogProcessorTests
{
    private readonly ICorrelationContext _correlationContext;
    private readonly CorrelationLogProcessor _processor;

    public CorrelationLogProcessorTests()
    {
        _correlationContext = Substitute.For<ICorrelationContext>();
        _processor = new CorrelationLogProcessor(_correlationContext);
    }

    [Fact]
    public void OnEnd_WithValidCorrelationId_ShouldAddCorrelationAttribute()
    {
        // Arrange
        var correlationId = "test-correlation-id";
        _correlationContext.GetCorrelationId().Returns(correlationId);

        var logRecord = CreateMockLogRecord();
        logRecord.Attributes = [];

        // Act
        _processor.OnEnd(logRecord);

        // Assert
        Assert.NotNull(logRecord.Attributes);
        Assert.Contains(logRecord.Attributes, attr =>
            attr.Key == "correlation_id" &&
            attr.Value?.ToString() == correlationId);
    }

    [Fact]
    public void OnEnd_WithEmptyCorrelationId_ShouldNotAddCorrelationAttribute()
    {
        // Arrange
        _correlationContext.GetCorrelationId().Returns(string.Empty);

        var logRecord = CreateMockLogRecord();
        var originalAttributesCount = logRecord.Attributes?.Count ?? 0;

        // Act
        _processor.OnEnd(logRecord);

        // Assert
        var currentAttributesCount = logRecord.Attributes?.Count ?? 0;
        Assert.Equal(originalAttributesCount, currentAttributesCount);
    }

    [Fact]
    public void OnEnd_WithNullCorrelationId_ShouldNotAddCorrelationAttribute()
    {
        // Arrange
        _correlationContext.GetCorrelationId().Returns((string?)null);

        var logRecord = CreateMockLogRecord();
        var originalAttributesCount = logRecord.Attributes?.Count ?? 0;

        // Act
        _processor.OnEnd(logRecord);

        // Assert
        var currentAttributesCount = logRecord.Attributes?.Count ?? 0;
        Assert.Equal(originalAttributesCount, currentAttributesCount);
    }

    [Fact]
    public void OnEnd_WithNullAttributes_ShouldInitializeAttributesAndAddCorrelation()
    {
        // Arrange
        var correlationId = "test-correlation-id";
        _correlationContext.GetCorrelationId().Returns(correlationId);

        var logRecord = CreateMockLogRecord();
        logRecord.Attributes = null;

        // Act
        _processor.OnEnd(logRecord);

        // Assert
        Assert.NotNull(logRecord.Attributes);
        Assert.Single(logRecord.Attributes);
        Assert.Contains(logRecord.Attributes, attr =>
            attr.Key == "correlation_id" &&
            attr.Value?.ToString() == correlationId);
    }

    [Fact]
    public void OnEnd_WithExistingAttributes_ShouldAppendCorrelationAttribute()
    {
        // Arrange
        var correlationId = "test-correlation-id";
        _correlationContext.GetCorrelationId().Returns(correlationId);

        var existingAttribute = new KeyValuePair<string, object?>("existing_key", "existing_value");
        var logRecord = CreateMockLogRecord();
        logRecord.Attributes = [existingAttribute];

        // Act
        _processor.OnEnd(logRecord);

        // Assert
        Assert.NotNull(logRecord.Attributes);
        Assert.Equal(2, logRecord.Attributes.Count);
        Assert.Contains(logRecord.Attributes, attr => attr.Key == "existing_key");
        Assert.Contains(logRecord.Attributes, attr =>
            attr.Key == "correlation_id" &&
            attr.Value?.ToString() == correlationId);
    }

    private static LogRecord CreateMockLogRecord()
    {
        var logRecordType = typeof(LogRecord);
        var instance = Activator.CreateInstance(logRecordType, true);
        return (LogRecord)instance!;
    }
}
