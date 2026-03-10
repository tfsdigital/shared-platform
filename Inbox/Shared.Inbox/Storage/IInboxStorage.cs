using Shared.Inbox.Models;

namespace Shared.Inbox.Storage;

public interface IInboxStorage
{
    Task<IReadOnlyList<InboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken);
    Task AddAsync(InboxMessage message, CancellationToken cancellationToken);
    Task<bool> IsAlreadyProcessedAsync(Guid integrationEventId, CancellationToken cancellationToken);
    Task UpdateMessageAsync(InboxMessage message, CancellationToken cancellationToken);
    Task CommitAsync(CancellationToken cancellationToken);
}
