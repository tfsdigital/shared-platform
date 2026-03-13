using Shared.Correlation.Context;

namespace Shared.Correlation.Tests.Context;

public class CorrelationContextTests
{
    [Fact]
    public void CorrelationId_WhenSet_ShouldReturnSameValue()
    {
        // Arrange
        var context = new CorrelationContext();
        var correlationId = "test-correlation-id-123";

        // Act
        context.SetCorrelationId(correlationId);
        var result = context.GetCorrelationId();

        // Assert
        Assert.Equal(correlationId, result);
    }

    [Fact]
    public void CorrelationId_WhenNotSet_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = new CorrelationContext();

        // Act & Assert
        Assert.Null(context.GetCorrelationId());
    }

    [Fact]
    public void CorrelationId_WhenSetToNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = new CorrelationContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => context.SetCorrelationId(null!));
    }

    [Fact]
    public void CorrelationId_WhenSetToEmptyString_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = new CorrelationContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => context.SetCorrelationId(""));
    }

    [Fact]
    public void CorrelationId_WhenSetToWhitespace_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = new CorrelationContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => context.SetCorrelationId("   "));
    }

    [Fact]
    public void CorrelationId_WhenSetWithValidValue_ShouldNotThrow()
    {
        // Arrange
        var context = new CorrelationContext();
        var correlationId = "valid-correlation-id";

        // Act & Assert
        var exception = Record.Exception(() => context.SetCorrelationId(correlationId));
        Assert.Null(exception);
        Assert.Equal(correlationId, context.GetCorrelationId());
    }

    [Fact]
    public void CorrelationId_WhenUpdated_ShouldReturnNewValue()
    {
        // Arrange
        var context = new CorrelationContext();
        var firstId = "first-correlation-id";
        var secondId = "second-correlation-id";

        // Act
        context.SetCorrelationId(firstId);
        var firstResult = context.GetCorrelationId();

        context.SetCorrelationId(secondId);
        var secondResult = context.GetCorrelationId();

        // Assert
        Assert.Equal(firstId, firstResult);
        Assert.Equal(secondId, secondResult);
        Assert.NotEqual(firstResult, secondResult);
    }

    [Fact]
    public void CorrelationContext_ShouldImplementICorrelationContext()
    {
        // Arrange & Act
        var context = new CorrelationContext();

        // Assert
        Assert.IsAssignableFrom<ICorrelationContext>(context);
    }

    [Fact]
    public void CorrelationId_WhenSetWithGuid_ShouldAcceptGuidFormat()
    {
        // Arrange
        var context = new CorrelationContext();
        var correlationId = Guid.NewGuid().ToString();

        // Act
        context.SetCorrelationId(correlationId);
        var result = context.GetCorrelationId();

        // Assert
        Assert.Equal(correlationId, result);
    }

    [Fact]
    public void CorrelationId_WhenSetWithSpecialCharacters_ShouldAcceptValue()
    {
        // Arrange
        var context = new CorrelationContext();
        var correlationId = "correlation-id-with-special-chars-123!@#";

        // Act
        context.SetCorrelationId(correlationId);
        var result = context.GetCorrelationId();

        // Assert
        Assert.Equal(correlationId, result);
    }

    [Fact]
    public void NewCorrelationId_ShouldGenerateAndSetCorrelationId()
    {
        // Arrange
        var context = new CorrelationContext();

        // Act
        var correlationId = context.NewCorrelationId();

        // Assert
        Assert.NotNull(correlationId);
        Assert.NotEmpty(correlationId);
        Assert.Equal(correlationId, context.GetCorrelationId());
    }

    [Fact]
    public void NewCorrelationId_WhenCalledTwice_ShouldReturnDifferentIds()
    {
        // Arrange
        var context = new CorrelationContext();

        // Act
        var first = context.NewCorrelationId();
        var second = context.NewCorrelationId();

        // Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public async Task CorrelationId_WhenAccessedFromDifferentThreads_ShouldMaintainThreadLocalValue()
    {
        // Arrange
        var context = new CorrelationContext();
        var correlationId1 = "thread-1-correlation-id";
        var correlationId2 = "thread-2-correlation-id";
        string? resultFromThread1 = null;
        string? resultFromThread2 = null;

        // Act
        var task1 = Task.Run(() =>
        {
            context.SetCorrelationId(correlationId1);
            // Use Task.Delay instead of Thread.Sleep for better async testing
            Task.Delay(TimeSpan.FromMilliseconds(50)).Wait();
            resultFromThread1 = context.GetCorrelationId();
        });

        var task2 = Task.Run(() =>
        {
            context.SetCorrelationId(correlationId2);
            // Use Task.Delay instead of Thread.Sleep for better async testing
            Task.Delay(TimeSpan.FromMilliseconds(50)).Wait();
            resultFromThread2 = context.GetCorrelationId();
        });

        await Task.WhenAll(task1, task2);

        // Assert
        Assert.Equal(correlationId1, resultFromThread1);
        Assert.Equal(correlationId2, resultFromThread2);
    }
}
