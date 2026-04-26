using Microsoft.Extensions.Logging.Abstractions;

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

    private static OutboxMessage CreateMessage() =>
        OutboxMessage.Create(
            "events",
            Guid.NewGuid(),
            new { Name = "Test" },
            DateTime.UtcNow);
}
