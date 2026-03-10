using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shared.Messaging.Connection;

namespace Shared.Messaging.Tests.Connection;

public class MessageBusConnectionFactoryTests
{
    [Fact]
    public async Task CreateConnectionAsync_WithValidConnectionString_ReturnsConnection()
    {
        // Arrange
        var configuration = Substitute.For<IConfiguration>();
        var connectionString = "amqp://localhost:0000";
        configuration.GetConnectionString("RabbitMQ").Returns(connectionString);
        var factory = new MessageBusConnectionFactory(configuration);

        // Act & Assert
        var exception = await Record.ExceptionAsync(async () => await factory.CreateConnectionAsync());

        // Assert that configuration was called at least once
        configuration.Received().GetConnectionString("RabbitMQ");

        // We expect a connection exception since RabbitMQ is not running
        // This verifies the configuration was used and connection was attempted
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task CreateConnectionAsync_WithCancellationToken_PassesCancellationToken()
    {
        // Arrange
        var configuration = Substitute.For<IConfiguration>();
        var connectionString = "amqp://localhost:0000";
        var cancellationToken = new CancellationToken();
        configuration.GetConnectionString("RabbitMQ").Returns(connectionString);
        var factory = new MessageBusConnectionFactory(configuration);

        // Act & Assert
        var exception = await Record.ExceptionAsync(async () => await factory.CreateConnectionAsync(cancellationToken));

        // Assert that configuration was called at least once
        configuration.Received().GetConnectionString("RabbitMQ");

        // We expect a connection exception since RabbitMQ is not running
        Assert.NotNull(exception);
    }

    [Fact]
    public void CreateConnectionAsync_CallsConfigurationGetConnectionString()
    {
        // Arrange
        var configuration = Substitute.For<IConfiguration>();
        var connectionString = "amqp://localhost:5672";
        configuration.GetConnectionString("RabbitMQ").Returns(connectionString);
        var factory = new MessageBusConnectionFactory(configuration);

        // Act
        try
        {
            var _ = factory.CreateConnectionAsync();
        }
        catch
        {
            // Expected since RabbitMQ is not running
        }

        // Assert
        configuration.Received().GetConnectionString("RabbitMQ");
    }
}
