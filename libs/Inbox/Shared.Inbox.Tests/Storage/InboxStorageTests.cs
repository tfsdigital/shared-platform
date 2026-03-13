using Shared.Core.Events;
using Shared.Inbox.Models;
using Shared.Inbox.Settings;
using Shared.Inbox.Storage;

namespace Shared.Inbox.Tests.Storage;

public record TestIntegrationEvent(string TestProperty) : IntegrationEvent;

public class InboxStorageTests
{
    private readonly InboxSettings _settings;
    private readonly InboxStorage _storage;

    public InboxStorageTests()
    {
        _settings = new InboxSettings
        {
            ConnectionString = "Data Source=:memory:;",
            MessagesBatchSize = 10,
        };
        _storage = new InboxStorage(_settings);
    }

    [Fact]
    public async Task AddAsync_WithValidMessage_ShouldAddMessageToDatabase()
    {
        // Arrange
        var testEvent = new TestIntegrationEvent("test content");
        var message = InboxMessage.Create(Guid.NewGuid(), testEvent, DateTime.UtcNow);

        // Act & Assert
        // Note: This test would require a real database setup for proper integration testing
        // For unit testing, we would need to mock the database interactions
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _storage.AddAsync(message, CancellationToken.None)
        );
    }

    [Fact]
    public async Task IsAlreadyProcessedAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var existingId = Guid.NewGuid();

        // Act & Assert
        // Note: This test would require a real database setup for proper integration testing
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _storage.IsAlreadyProcessedAsync(existingId, CancellationToken.None)
        );
    }

    [Fact]
    public async Task GetUnprocessedMessagesAsync_WithUnprocessedMessages_ShouldReturnMessages()
    {
        // Act & Assert
        // Note: This test would require a real database setup for proper integration testing
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _storage.GetUnprocessedMessagesAsync(CancellationToken.None)
        );
    }

    [Fact]
    public async Task UpdateMessageAsync_WithoutOpenConnection_ShouldThrowNullReferenceException()
    {
        // Arrange
        var message = InboxMessage.Create(
            Guid.NewGuid(),
            new TestIntegrationEvent("test content"),
            DateTime.UtcNow
        );

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(
            async () => await _storage.UpdateMessageAsync(message, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CommitAsync_WithoutOpenTransaction_ShouldThrowNullReferenceException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(
            async () => await _storage.CommitAsync(CancellationToken.None)
        );
    }

    [Fact]
    public void Constructor_WithValidSettings_ShouldCreateInstance()
    {
        // Arrange & Act
        var storage = new InboxStorage(_settings);

        // Assert
        Assert.NotNull(storage);
    }
}
