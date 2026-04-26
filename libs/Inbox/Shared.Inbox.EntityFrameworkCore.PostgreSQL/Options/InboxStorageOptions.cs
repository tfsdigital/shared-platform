namespace Shared.Inbox.EntityFrameworkCore.PostgreSQL.Options;

public class InboxStorageOptions
{
    public string Schema { get; set; } = "public";
    public string TableName { get; set; } = "inbox_messages";

    public void Validate()
    {
        var errors = new List<string>();

        if (!PostgreSqlIdentifier.IsValid(Schema))
            errors.Add("Schema must be a valid PostgreSQL identifier.");

        if (!PostgreSqlIdentifier.IsValid(TableName))
            errors.Add("TableName must be a valid PostgreSQL identifier.");

        if (errors.Count > 0)
            throw new ArgumentException(
                $"Invalid {nameof(InboxStorageOptions)}: {string.Join(" ", errors)}");
    }
}
