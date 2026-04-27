using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shared.Events;

namespace Shared.Publishing.Tests;

public class EventHandlerWrapperImplementationTests
{
    public class TestEvent : IEventBase
    {
        public Guid MessageId { get; } = Guid.NewGuid();
        public DateTime OccurredOnUtc => DateTime.UtcNow;
    }

    [Fact]
    public async Task Handle_WithNoHandlers_ShouldCallPublishWithEmptyExecutors()
    {
        // Arrange
        var wrapper = new EventHandlerWrapperImplementation<TestEvent>();
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var testEvent = new TestEvent();

        IEnumerable<EventHandlerExecutor>? capturedExecutors = null;
        Task Publish(IEnumerable<EventHandlerExecutor> executors, IEventBase ev, CancellationToken ct)
        {
            capturedExecutors = executors;
            return Task.CompletedTask;
        }

        // Act
        await wrapper.Handle(testEvent, serviceProvider, Publish, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedExecutors);
        Assert.Empty(capturedExecutors);
    }

    [Fact]
    public async Task Handle_WithOneHandler_ShouldCreateOneExecutor()
    {
        // Arrange
        var wrapper = new EventHandlerWrapperImplementation<TestEvent>();
        var serviceCollection = new ServiceCollection();
        var handler = Substitute.For<IEventHandler<TestEvent>>();
        serviceCollection.AddSingleton(handler);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var testEvent = new TestEvent();

        IEnumerable<EventHandlerExecutor>? capturedExecutors = null;
        Task Publish(IEnumerable<EventHandlerExecutor> executors, IEventBase ev, CancellationToken ct)
        {
            capturedExecutors = executors;
            return Task.CompletedTask;
        }

        // Act
        await wrapper.Handle(testEvent, serviceProvider, Publish, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedExecutors);
        Assert.Single(capturedExecutors);
    }

    [Fact]
    public async Task Handle_WithMultipleHandlers_ShouldCreateExecutorForEachHandler()
    {
        // Arrange
        var wrapper = new EventHandlerWrapperImplementation<TestEvent>();
        var serviceCollection = new ServiceCollection();
        var handler1 = Substitute.For<IEventHandler<TestEvent>>();
        var handler2 = Substitute.For<IEventHandler<TestEvent>>();
        var handler3 = Substitute.For<IEventHandler<TestEvent>>();
        serviceCollection.AddSingleton(handler1);
        serviceCollection.AddSingleton(handler2);
        serviceCollection.AddSingleton(handler3);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var testEvent = new TestEvent();

        IEnumerable<EventHandlerExecutor>? capturedExecutors = null;
        Task Publish(IEnumerable<EventHandlerExecutor> executors, IEventBase ev, CancellationToken ct)
        {
            capturedExecutors = executors;
            return Task.CompletedTask;
        }

        // Act
        await wrapper.Handle(testEvent, serviceProvider, Publish, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedExecutors);
        Assert.Equal(3, capturedExecutors.Count());
    }
}
