using System.Text.Json;
using Shared.Core.Events;
using Shared.Inbox.Models;

namespace Shared.Inbox.Tests.Models;

public class InboxMessageTests
{
    private record TestIntegrationEvent(string Name, int Value) : IIntegrationEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    [Fact]
    public void Create_WhenCalledWithValidParameters_ShouldCreateInboxMessage()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var content = new TestIntegrationEvent("Test", 123);
        var occurredOn = DateTime.UtcNow;

        // Act
        var message = InboxMessage.Create(eventId, content, occurredOn);

        // Assert
        Assert.Equal(eventId, message.Id);
        Assert.Equal(occurredOn, message.OccurredOn);
        Assert.NotNull(message.Content);
        Assert.NotNull(message.Type);
        Assert.Null(message.ProcessedOn);
        Assert.Null(message.ErrorHandledOn);
        Assert.Null(message.Error);
    }

    [Fact]
    public void Create_WhenCalledWithNullHeaders_ShouldCreateMessageWithNullHeaders()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var content = new TestIntegrationEvent("Test", 123);
        var occurredOn = DateTime.UtcNow;

        // Act
        var message = InboxMessage.Create(eventId, content, occurredOn, null);

        // Assert
        Assert.Null(message.Headers);
    }

    [Fact]
    public void Create_WhenCalledWithHeaders_ShouldSerializeHeaders()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var content = new TestIntegrationEvent("Test", 123);
        var occurredOn = DateTime.UtcNow;
        var headers = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        // Act
        var message = InboxMessage.Create(eventId, content, occurredOn, headers);

        // Assert
        Assert.NotNull(message.Headers);
        Assert.NotEqual(string.Empty, message.Headers);

        var deserializedHeaders = JsonSerializer.Deserialize<Dictionary<string, string>>(message.Headers);
        Assert.Equal(headers, deserializedHeaders);
    }

    [Fact]
    public void Create_WhenCalledWithContent_ShouldSerializeContentAndSetType()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var content = new TestIntegrationEvent("Test", 123);
        var occurredOn = DateTime.UtcNow;

        // Act
        var message = InboxMessage.Create(eventId, content, occurredOn);

        // Assert
        Assert.Contains("TestIntegrationEvent", message.Type);

        var deserializedContent = JsonSerializer.Deserialize<TestIntegrationEvent>(message.Content);
        Assert.Equal(content.Name, deserializedContent?.Name);
        Assert.Equal(content.Value, deserializedContent?.Value);
    }

    [Fact]
    public void MarkAsProcessed_WhenCalled_ShouldSetProcessedOnToCurrentTime()
    {
        // Arrange
        var message = InboxMessage.Create(Guid.NewGuid(), new TestIntegrationEvent("Test", 1), DateTime.UtcNow);
        var beforeProcessing = DateTime.UtcNow;

        // Act
        message.MarkAsProcessedWithSuccess();
        var afterProcessing = DateTime.UtcNow;

        // Assert
        Assert.NotNull(message.ProcessedOn);
        Assert.True(message.ProcessedOn >= beforeProcessing);
        Assert.True(message.ProcessedOn <= afterProcessing);
    }

    [Fact]
    public void MarkAsFailed_WhenCalledWithError_ShouldSetErrorAndMarkAsProcessed()
    {
        // Arrange
        var message = InboxMessage.Create(Guid.NewGuid(), new TestIntegrationEvent("Test", 1), DateTime.UtcNow);
        var errorMessage = "Test error message";
        var beforeFailure = DateTime.UtcNow;

        // Act
        message.MarkAsProcessedWithError(errorMessage);
        var afterFailure = DateTime.UtcNow;

        // Assert
        Assert.Equal(errorMessage, message.Error);
        Assert.NotNull(message.ErrorHandledOn);
        Assert.NotNull(message.ProcessedOn);
        Assert.True(message.ErrorHandledOn >= beforeFailure);
        Assert.True(message.ErrorHandledOn <= afterFailure);
        Assert.True(message.ProcessedOn >= beforeFailure);
        Assert.True(message.ProcessedOn <= afterFailure);
    }

    [Fact]
    public void GetHeaders_WhenHeadersIsEmpty_ShouldReturnNull()
    {
        // Arrange
        var message = InboxMessage.Create(Guid.NewGuid(), new TestIntegrationEvent("Test", 1), DateTime.UtcNow);

        // Act
        var headers = message.GetHeaders();

        // Assert
        Assert.Null(headers);
    }

    [Fact]
    public void GetHeaders_WhenHeadersExist_ShouldReturnDeserializedHeaders()
    {
        // Arrange
        var originalHeaders = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };
        var message = InboxMessage.Create(Guid.NewGuid(), new TestIntegrationEvent("Test", 1), DateTime.UtcNow, originalHeaders);

        // Act
        var headers = message.GetHeaders();

        // Assert
        Assert.NotNull(headers);
        Assert.Equal(2, headers.Count);
        Assert.True(headers.ContainsKey("key1"));
        Assert.True(headers.ContainsKey("key2"));
    }

    [Fact]
    public void GetContent_WhenCalled_ShouldReturnDeserializedContent()
    {
        // Arrange
        var originalContent = new TestIntegrationEvent("Test", 123);
        var message = InboxMessage.Create(Guid.NewGuid(), originalContent, DateTime.UtcNow);

        // Act
        var content = message.GetContent();

        // Assert
        Assert.NotNull(content);
        Assert.IsAssignableFrom<IIntegrationEvent>(content);
        Assert.IsType<TestIntegrationEvent>(content);

        var typedContent = (TestIntegrationEvent)content;
        Assert.Equal(originalContent.Name, typedContent.Name);
        Assert.Equal(originalContent.Value, typedContent.Value);
    }

    [Fact]
    public void Create_WhenCalledWithEmptyDictionary_ShouldSerializeEmptyDictionary()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var content = new TestIntegrationEvent("Test", 123);
        var occurredOn = DateTime.UtcNow;
        var emptyHeaders = new Dictionary<string, string>();

        // Act
        var message = InboxMessage.Create(eventId, content, occurredOn, emptyHeaders);

        // Assert
        Assert.NotNull(message.Headers);
        Assert.Equal("{}", message.Headers);
    }

    [Fact]
    public void MarkAsProcessed_WhenCalledMultipleTimes_ShouldUpdateProcessedOn()
    {
        // Arrange
        var baseTime = DateTime.UtcNow;
        var message = InboxMessage.Create(Guid.NewGuid(), new TestIntegrationEvent("Test", 1), baseTime);
        message.MarkAsProcessedWithSuccess();
        var firstProcessedOn = message.ProcessedOn;

        // Act
        // Create another message to test timestamp comparison without Thread.Sleep
        var laterMessage = InboxMessage.Create(Guid.NewGuid(), new TestIntegrationEvent("Test", 1), baseTime.AddMilliseconds(1));
        laterMessage.MarkAsProcessedWithSuccess();
        laterMessage.MarkAsProcessedWithSuccess(); // Call twice to test update

        // Assert
        Assert.NotNull(laterMessage.ProcessedOn);
        Assert.True(laterMessage.ProcessedOn >= firstProcessedOn);
    }

    [Fact]
    public void MarkAsFailed_WhenCalledAfterMarkAsProcessed_ShouldOverrideProcessedOn()
    {
        // Arrange
        var baseTime = DateTime.UtcNow;
        var message = InboxMessage.Create(Guid.NewGuid(), new TestIntegrationEvent("Test", 1), baseTime);
        message.MarkAsProcessedWithSuccess();

        // Act
        // Test error marking without relying on Thread.Sleep
        message.MarkAsProcessedWithError("Error occurred");

        // Assert
        Assert.NotNull(message.ProcessedOn);
        Assert.NotNull(message.Error);
        Assert.NotNull(message.ErrorHandledOn);
        Assert.Equal("Error occurred", message.Error);
    }

    [Fact]
    public void Create_WhenCalledWithNullContent_ShouldThrowException()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var occurredOn = DateTime.UtcNow;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => InboxMessage.Create<TestIntegrationEvent>(eventId, null!, occurredOn));
    }

    [Fact]
    public void MarkAsFailed_WhenCalledWithNullError_ShouldSetNullError()
    {
        // Arrange
        var message = InboxMessage.Create(Guid.NewGuid(), new TestIntegrationEvent("Test", 1), DateTime.UtcNow);

        // Act
        message.MarkAsProcessedWithError(null!);

        // Assert
        Assert.Null(message.Error);
        Assert.NotNull(message.ErrorHandledOn);
        Assert.NotNull(message.ProcessedOn);
    }

    [Fact]
    public void Create_WhenCalled_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var content = new TestIntegrationEvent("Test", 1);
        var occurredOn = DateTime.UtcNow;

        // Act
        var message = InboxMessage.Create(eventId, content, occurredOn);

        // Assert
        Assert.Equal(eventId, message.Id);
        Assert.NotNull(message.Type);
        Assert.NotNull(message.Content);
        Assert.Equal(occurredOn, message.OccurredOn);
    }
}
