using Shared.Core.Events;

namespace Shared.Publishing.Tests;

public class EventHandlerExecutorTests
{
    private class TestEvent : IEventBase
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }

    private class TestEventHandler
    {
        public bool WasCalled { get; private set; }
        public IEventBase? ReceivedEvent { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }

        public Task HandleAsync(IEventBase eventBase, CancellationToken cancellationToken)
        {
            WasCalled = true;
            ReceivedEvent = eventBase;
            ReceivedCancellationToken = cancellationToken;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public void EventHandlerExecutor_WithValidParameters_CreatesCorrectInstance()
    {
        // Arrange
        var handler = new TestEventHandler();
        var callback = handler.HandleAsync;

        // Act
        var executor = new EventHandlerExecutor(handler, callback);

        // Assert
        Assert.Equal(handler, executor.HandlerInstance);
        Assert.Equal(callback, executor.HandlerCallback);
    }

    [Fact]
    public async Task EventHandlerExecutor_ExecuteCallback_CallsHandlerMethod()
    {
        // Arrange
        var handler = new TestEventHandler();
        var callback = handler.HandleAsync;
        var executor = new EventHandlerExecutor(handler, callback);
        var testEvent = new TestEvent();
        var cancellationToken = new CancellationToken();

        // Act
        await executor.HandlerCallback(testEvent, cancellationToken);

        // Assert
        Assert.True(handler.WasCalled);
        Assert.Equal(testEvent, handler.ReceivedEvent);
        Assert.Equal(cancellationToken, handler.ReceivedCancellationToken);
    }

    [Fact]
    public void EventHandlerExecutor_Deconstruct_ReturnsCorrectValues()
    {
        // Arrange
        var handler = new TestEventHandler();
        var callback = handler.HandleAsync;
        var executor = new EventHandlerExecutor(handler, callback);

        // Act
        var (handlerInstance, handlerCallback) = executor;

        // Assert
        Assert.Equal(handler, handlerInstance);
        Assert.Equal(callback, handlerCallback);
    }
}
