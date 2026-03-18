using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shared.Core.Events;
using Shared.Publishing.Extensions;

namespace Shared.Publishing.Tests;

public class EventPublisherTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly EventPublisher _eventPublisher;

    public EventPublisherTests()
    {
        var serviceCollection = new ServiceCollection();
        _serviceProvider = serviceCollection.BuildServiceProvider();
        _eventPublisher = new EventPublisher(_serviceProvider);
    }

    [Fact]
    public async Task Publish_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Arrange
        TestEvent? nullEvent = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _eventPublisher.Publish(nullEvent!, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Publish_WithValidEvent_ShouldNotThrow()
    {
        // Arrange
        var testEvent = new TestEvent { Message = "Test message" };

        // Act & Assert
        var exception = await Record.ExceptionAsync(
            () => _eventPublisher.Publish(testEvent, CancellationToken.None)
        );

        Assert.Null(exception);
    }

    [Fact]
    public async Task Publish_WithCancellationToken_ShouldNotThrow()
    {
        // Arrange
        var testEvent = new TestEvent { Message = "Test message" };
        var cancellationToken = new CancellationToken(false);

        // Act
        var exception = await Record.ExceptionAsync(
            async () => await _eventPublisher.Publish(testEvent, cancellationToken)
        );

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task Publish_WithEventHandler_ShouldCallHandler()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var eventHandler = Substitute.For<IEventHandler<TestEvent>>();
        serviceCollection.AddSingleton(eventHandler);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var publisher = new EventPublisher(serviceProvider);

        var testEvent = new TestEvent { Message = "Test message" };

        // Act
        await publisher.Publish(testEvent, CancellationToken.None);

        // Assert
        await eventHandler.Received(1).HandleAsync(testEvent, CancellationToken.None);
    }

    [Fact]
    public async Task Publish_WithMultipleHandlers_ShouldCallAllHandlers()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var eventHandler1 = Substitute.For<IEventHandler<TestEvent>>();
        var eventHandler2 = Substitute.For<IEventHandler<TestEvent>>();

        serviceCollection.AddSingleton(eventHandler1);
        serviceCollection.AddSingleton(eventHandler2);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var publisher = new EventPublisher(serviceProvider);

        var testEvent = new TestEvent { Message = "Test message" };

        // Act
        await publisher.Publish(testEvent, CancellationToken.None);

        // Assert
        await eventHandler1.Received(1).HandleAsync(testEvent, CancellationToken.None);
        await eventHandler2.Received(1).HandleAsync(testEvent, CancellationToken.None);
    }

    public class TestEvent : IEventBase
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Message { get; set; } = string.Empty;
        public DateTime OccurredOnUtc => DateTime.UtcNow;
    }
}

public class PublishingExtensionsTests
{
    [Fact]
    public void AddEventPublishing_ShouldRegisterEventPublisherAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEventPublishing();

        // Assert
        var descriptor = Assert.Single(services, sd => sd.ServiceType == typeof(IEventPublisher));
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        Assert.Equal(typeof(EventPublisher), descriptor.ImplementationType);
    }

    [Fact]
    public void AddEventPublishing_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddEventPublishing();

        // Assert
        Assert.Same(services, result);
    }
}
