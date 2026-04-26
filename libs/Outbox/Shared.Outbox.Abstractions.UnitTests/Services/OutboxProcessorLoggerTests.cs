using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

using Shared.Outbox.Abstractions.Models;
using Shared.Outbox.Abstractions.Services;

namespace Shared.Outbox.Abstractions.UnitTests.Services;

public class OutboxProcessorLoggerTests
{
    [Fact]
    public void LogMethods_WithoutModule_DoNotThrow()
    {
        var message = CreateMessage();

        OutboxProcessorLogger.LogPublished(NullLogger<OutboxProcessorLoggerTests>.Instance, null, message);
        OutboxProcessorLogger.LogCancelled(NullLogger<OutboxProcessorLoggerTests>.Instance, null);
        OutboxProcessorLogger.LogFailed(
            NullLogger<OutboxProcessorLoggerTests>.Instance,
            null,
            new InvalidOperationException("failed"),
            message);

        Assert.Equal("events", message.Destination);
    }

    [Fact]
    public void LogMethods_WithModule_DoNotThrow()
    {
        var message = CreateMessage();

        OutboxProcessorLogger.LogPublished(NullLogger<OutboxProcessorLoggerTests>.Instance, "orders", message);
        OutboxProcessorLogger.LogCancelled(NullLogger<OutboxProcessorLoggerTests>.Instance, "orders");
        OutboxProcessorLogger.LogFailed(
            NullLogger<OutboxProcessorLoggerTests>.Instance,
            "orders",
            new InvalidOperationException("failed"),
            message);

        Assert.Equal("events", message.Destination);
    }

    [Fact]
    public void LogMethods_WhenLoggerIsEnabled_WriteAllBranches()
    {
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<OutboxProcessorLoggerTests>>();
        var message = CreateMessage();
        logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information).Returns(true);
        logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error).Returns(true);

        OutboxProcessorLogger.LogPublished(logger, null, message);
        OutboxProcessorLogger.LogPublished(logger, "orders", message);
        OutboxProcessorLogger.LogCancelled(logger, null);
        OutboxProcessorLogger.LogCancelled(logger, "orders");
        OutboxProcessorLogger.LogFailed(logger, null, new InvalidOperationException("failed"), message);
        OutboxProcessorLogger.LogFailed(logger, "orders", new InvalidOperationException("failed"), message);

        logger.Received(4).IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information);
        logger.Received(2).IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error);
    }

    private static OutboxMessage CreateMessage() =>
        OutboxMessage.Create(
            "events",
            Guid.NewGuid(),
            new { Name = "Test" },
            DateTime.UtcNow);
}
