using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Shared.Messaging.Connection;

public class MessageBusConnectionFactory(IConfiguration configuration) : IMessageBusConnectionFactory
{
    public async Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri(configuration.GetConnectionString("RabbitMQ")!)
        };

        return await factory.CreateConnectionAsync(cancellationToken);
    }
}