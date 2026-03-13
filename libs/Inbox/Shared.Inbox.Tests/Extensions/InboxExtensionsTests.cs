using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shared.Core.Events;
using Shared.Correlation.Context;
using Shared.Inbox.Extensions;
using Shared.Messaging.Connection;
using Shared.Publishing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shared.Inbox.Tests.Extensions;

public class InboxExtensionsTests
{
    private record TestInboxIntegrationEvent(string Data) : IIntegrationEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    [Fact]
    public void AddInboxConsumer_ShouldRegisterHostedService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Substitute.For<IMessageBusConnectionFactory>());

        // Act
        services.AddInboxConsumer<TestInboxIntegrationEvent>(
            moduleName: "test-module",
            exchangeName: "test-exchange",
            connectionString: "Host=localhost;Database=test",
            intervalInSeconds: 5,
            messagesBatchSize: 10
        );

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IHostedService));
        Assert.NotNull(descriptor);
    }

    [Fact]
    public void AddInboxConsumer_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Substitute.For<IMessageBusConnectionFactory>());

        // Act
        var result = services.AddInboxConsumer<TestInboxIntegrationEvent>(
            moduleName: "test-module",
            exchangeName: "test-exchange",
            connectionString: "Host=localhost;Database=test",
            intervalInSeconds: 5,
            messagesBatchSize: 10
        );

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddInboxProcessor_ShouldRegisterHostedService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Substitute.For<ICorrelationContext>());
        services.AddSingleton(Substitute.For<IEventPublisher>());

        // Act
        services.AddInboxProcessor(
            moduleName: "test-module",
            connectionString: "Host=localhost;Database=test",
            intervalInSeconds: 5,
            messagesBatchSize: 10
        );

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IHostedService));
        Assert.NotNull(descriptor);
    }

    [Fact]
    public void AddInboxProcessor_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Substitute.For<ICorrelationContext>());
        services.AddSingleton(Substitute.For<IEventPublisher>());

        // Act
        var result = services.AddInboxProcessor(
            moduleName: "test-module",
            connectionString: "Host=localhost;Database=test",
            intervalInSeconds: 5,
            messagesBatchSize: 10
        );

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddInboxConsumer_WhenProviderIsBuilt_ShouldResolveHostedService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Substitute.For<IMessageBusConnectionFactory>());

        services.AddInboxConsumer<TestInboxIntegrationEvent>(
            moduleName: "test-module",
            exchangeName: "test-exchange",
            connectionString: "Host=localhost;Database=test",
            intervalInSeconds: 5,
            messagesBatchSize: 10
        );

        // Act
        var provider = services.BuildServiceProvider();
        var hostedServices = provider.GetServices<IHostedService>().ToList();

        // Assert
        Assert.NotEmpty(hostedServices);
    }

    [Fact]
    public void AddInboxProcessor_WhenProviderIsBuilt_ShouldResolveHostedService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Substitute.For<ICorrelationContext>());
        services.AddSingleton(Substitute.For<IEventPublisher>());

        services.AddInboxProcessor(
            moduleName: "test-module",
            connectionString: "Host=localhost;Database=test",
            intervalInSeconds: 5,
            messagesBatchSize: 10
        );

        // Act
        var provider = services.BuildServiceProvider();
        var hostedServices = provider.GetServices<IHostedService>().ToList();

        // Assert
        Assert.NotEmpty(hostedServices);
    }
}
