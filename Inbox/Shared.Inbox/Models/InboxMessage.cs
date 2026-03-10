using System.Text.Json;
using Shared.Core.Events;
using Shared.Core.Identification;

namespace Shared.Inbox.Models;

public class InboxMessage
{
    private InboxMessage(Guid id, string type, string content, DateTime occurredOn, string? headers)
    {
        Id = id;
        Type = type;
        Content = content;
        OccurredOn = occurredOn;
        Headers = headers;
    }

    private InboxMessage()
    {
    }

    public Guid Id { get; init; } = IdGenerator.CreateSequential();
    public string? Headers { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime OccurredOn { get; init; }
    public DateTime? ProcessedOn { get; private set; }
    public DateTime? ErrorHandledOn { get; private set; }
    public string? Error { get; private set; }

    public string GetTypeName()
    {
        ReadOnlySpan<char> span = Type;

        span = span[..span.IndexOf(',')].Trim();

        return span[(span.LastIndexOf('.') + 1)..].ToString();
    }

    public void MarkAsProcessedWithSuccess()
    {
        ProcessedOn = DateTime.UtcNow;
        Error = null;
        ErrorHandledOn = null;
    }

    public void MarkAsProcessedWithError(string error)
    {
        ProcessedOn = DateTime.UtcNow;
        ErrorHandledOn = DateTime.UtcNow;
        Error = error;
    }

    public static InboxMessage Create<TContent>(
        Guid eventId, TContent content, DateTime occurredOn, IDictionary<string, string>? headersDictionary = null)
    {
        string? headersJson = null;

        if (headersDictionary is not null)
        {
            headersJson = JsonSerializer.Serialize(headersDictionary);
        }

        var contentType = content!.GetType().FullName + ", " + content.GetType().Assembly.GetName().Name;
        var contentJson = JsonSerializer.Serialize(content);

        return new InboxMessage(eventId, contentType, contentJson, occurredOn, headersJson);
    }

    public IIntegrationEvent GetContent()
    {
        var contentType = System.Type.GetType(Type)!;
        var content = JsonSerializer.Deserialize(Content, contentType) as IIntegrationEvent;

        return content!;
    }

    public Dictionary<string, string>? GetHeaders()
    {
        return string.IsNullOrEmpty(Headers) ?
            null : JsonSerializer.Deserialize<Dictionary<string, string>>(Headers);
    }
}
