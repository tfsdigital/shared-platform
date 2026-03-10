using RabbitMQ.Client;

namespace Shared.Messaging.Connection;

public interface IMessageBusConnectionFactory
{
    Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
