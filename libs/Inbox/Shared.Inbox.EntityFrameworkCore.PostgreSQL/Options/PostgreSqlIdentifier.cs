namespace Shared.Inbox.EntityFrameworkCore.PostgreSQL.Options;

internal static class PostgreSqlIdentifier
{
    internal static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (!IsIdentifierStart(value[0]))
            return false;

        return value.All(IsIdentifierPart);
    }

    internal static string Quote(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException($"Invalid PostgreSQL identifier: {value}", nameof(value));

        return $"\"{value}\"";
    }

    private static bool IsIdentifierStart(char value) =>
        value == '_' || char.IsAsciiLetter(value);

    private static bool IsIdentifierPart(char value) =>
        IsIdentifierStart(value) || char.IsAsciiDigit(value);
}
