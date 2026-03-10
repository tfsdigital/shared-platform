using Dapper;
using Npgsql;
using Shared.Inbox.Models;
using Shared.Inbox.Settings;

namespace Shared.Inbox.Storage;

public class InboxStorage(InboxSettings settings) : IInboxStorage
{
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;

    public async Task<IReadOnlyList<InboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken)
    {
        _connection = new NpgsqlConnection(settings.ConnectionString);
        await _connection.OpenAsync(cancellationToken);

        _transaction = await _connection.BeginTransactionAsync(cancellationToken);

        const string sql =
            """
            SELECT * 
            FROM "InboxMessages"
            WHERE "ProcessedOn" IS NULL 
            ORDER BY "OccurredOn" 
            LIMIT @MessagesBatchSize
            FOR UPDATE
            """;

        var messages = await _connection.QueryAsync<InboxMessage>(
            sql,
            new { settings.MessagesBatchSize },
            _transaction);

        return messages.AsList();
    }

    public async Task AddAsync(InboxMessage message, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(settings.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
        INSERT INTO "InboxMessages" ("Id", "OccurredOn", "Type", "Content", "Headers")
        VALUES (@Id, @OccurredOn, @Type, @Content::json, @Headers::json)
        """;

        await connection.ExecuteAsync(sql, message);
    }

    public async Task<bool> IsAlreadyProcessedAsync(Guid integrationEventId, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(settings.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """SELECT COUNT(1) FROM "InboxMessages" WHERE "Id" = @Id""";

        return await connection.ExecuteScalarAsync<bool>(sql, new { Id = integrationEventId });
    }

    public async Task UpdateMessageAsync(InboxMessage message, CancellationToken cancellationToken)
    {
        const string sql =
            """
            UPDATE "InboxMessages" 
            SET "ProcessedOn" = @ProcessedOn, 
                "Error" = @Error, 
                "ErrorHandledOn" = @ErrorHandledOn
            WHERE "Id" = @Id;
            """;

        await _connection!.ExecuteAsync(sql, new
        {
            message.ProcessedOn,
            message.Error,
            message.ErrorHandledOn,
            message.Id
        }, _transaction);
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _transaction!.CommitAsync(cancellationToken);

        await _transaction.DisposeAsync();
        await _connection!.DisposeAsync();
    }
}