using Microsoft.Extensions.Logging.Abstractions;

using Shared.Messaging.RabbitMQ.Connection;
using Shared.Messaging.RabbitMQ.Options;

using MsOptions = Microsoft.Extensions.Options.Options;

namespace Shared.Messaging.RabbitMQ.UnitTests.Connection;

public class PersistentRabbitMqConnectionTests
{
    [Fact]
    public void IsConnected_WhenConnectionWasNotCreated_ReturnsFalse()
    {
        var connection = CreateConnection("amqp://localhost");

        Assert.False(connection.IsConnected);
    }

    [Fact]
    public async Task EnsureConnectedAsync_WhenConnectionStringIsInvalid_Throws()
    {
        var connection = CreateConnection("not a uri");

        await Assert.ThrowsAsync<UriFormatException>(
            () => connection.EnsureConnectedAsync(CancellationToken.None));
    }

    [Fact]
    public async Task CreateChannelAsync_WhenConnectionStringIsInvalid_Throws()
    {
        var connection = CreateConnection("not a uri");

        await Assert.ThrowsAsync<UriFormatException>(
            () => connection.CreateChannelAsync(cancellationToken: CancellationToken.None));
    }

    private static PersistentRabbitMqConnection CreateConnection(string connectionString) =>
        new(
            MsOptions.Create(new RabbitMqOptions { ConnectionString = connectionString }),
            NullLogger<PersistentRabbitMqConnection>.Instance);
}
