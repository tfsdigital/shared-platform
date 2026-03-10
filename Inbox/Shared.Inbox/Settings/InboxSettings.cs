namespace Shared.Inbox.Settings;

public record InboxSettings
{
    public string ConnectionString { get; init; } = string.Empty;
    public int IntervalInSeconds { get; init; }
    public int MessagesBatchSize { get; init; }
}
