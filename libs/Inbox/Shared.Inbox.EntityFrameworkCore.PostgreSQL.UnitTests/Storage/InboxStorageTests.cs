using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

using Shared.Inbox.Abstractions.Database;
using Shared.Inbox.Abstractions.Models;
using Shared.Inbox.EntityFrameworkCore.PostgreSQL.Options;
using Shared.Inbox.EntityFrameworkCore.PostgreSQL.Storage;

using MsOptions = Microsoft.Extensions.Options.Options;

namespace Shared.Inbox.EntityFrameworkCore.PostgreSQL.UnitTests.Storage;

public class InboxStorageTests
{
    [Fact]
    public async Task TryRegisterAsync_WhenProviderDoesNotSupportRawSql_Throws()
    {
        await using var context = CreateContext();
        var storage = new InboxStorage<TestInboxDbContext>(
            context,
            MsOptions.Create(new InboxStorageOptions()),
            NullLogger<InboxStorage<TestInboxDbContext>>.Instance);
        var message = InboxMessage.Create("message-1", "consumer");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => storage.TryRegisterAsync(message, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_WhenProviderDoesNotSupportRawSql_Throws()
    {
        await using var context = CreateContext();
        var storage = new InboxStorage<TestInboxDbContext>(
            context,
            MsOptions.Create(new InboxStorageOptions()),
            NullLogger<InboxStorage<TestInboxDbContext>>.Instance);
        var message = InboxMessage.Create("message-1", "consumer");
        message.MarkAsProcessedWithError("failed");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => storage.UpdateAsync(message, CancellationToken.None));
    }

    private static TestInboxDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestInboxDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestInboxDbContext(options);
    }

    private sealed class TestInboxDbContext(DbContextOptions<TestInboxDbContext> options)
        : DbContext(options), IInboxDbContext
    {
        public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    }
}
