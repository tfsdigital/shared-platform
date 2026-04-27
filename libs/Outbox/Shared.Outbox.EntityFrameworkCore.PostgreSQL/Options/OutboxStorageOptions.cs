namespace Shared.Outbox.EntityFrameworkCore.PostgreSQL.Options;

public record OutboxStorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Schema { get; set; } = "public";
    public string TableName { get; set; } = "OutboxMessages";

    public void Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(ConnectionString))
            errors.Add("ConnectionString is required.");

        if (!PostgreSqlIdentifier.IsValid(Schema))
            errors.Add("Schema must be a valid PostgreSQL identifier.");

        if (!PostgreSqlIdentifier.IsValid(TableName))
            errors.Add("TableName must be a valid PostgreSQL identifier.");

        if (errors.Count > 0)
            throw new ArgumentException(
                $"Invalid {nameof(OutboxStorageOptions)}: {string.Join(" ", errors)}");
    }
}
