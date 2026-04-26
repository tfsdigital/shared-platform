using Microsoft.Extensions.Logging.Abstractions;

using Shared.Inbox.Abstractions.Logging;
using Shared.Inbox.Abstractions.Models;

namespace Shared.Inbox.Abstractions.UnitTests.Logging;

public class InboxStorageLoggerTests
{
    [Fact]
    public void LogMethods_DoNotThrow()
    {
        var message = InboxMessage.Create("message-1", "consumer");

        InboxStorageLogger.LogRegistered(NullLogger<InboxStorageLoggerTests>.Instance, message);
        InboxStorageLogger.LogDuplicate(NullLogger<InboxStorageLoggerTests>.Instance, message);
    }
}
