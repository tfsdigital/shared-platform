using Shared.Outbox.EntityFrameworkCore.PostgreSQL.Options;

namespace Shared.Outbox.EntityFrameworkCore.PostgreSQL.UnitTests.Options;

public class PostgreSqlIdentifierTests
{
    [Theory]
    [InlineData("outbox")]
    [InlineData("_outbox")]
    [InlineData("outbox_2026")]
    [InlineData("Outbox")]
    public void IsValid_WithValidIdentifier_ReturnsTrue(string value)
    {
        Assert.True(PostgreSqlIdentifier.IsValid(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1outbox")]
    [InlineData("outbox-items")]
    [InlineData("outbox.items")]
    [InlineData("outbox items")]
    public void IsValid_WithInvalidIdentifier_ReturnsFalse(string? value)
    {
        Assert.False(PostgreSqlIdentifier.IsValid(value));
    }

    [Fact]
    public void Quote_WithValidIdentifier_ReturnsQuotedIdentifier()
    {
        var result = PostgreSqlIdentifier.Quote("outbox_2026");

        Assert.Equal("\"outbox_2026\"", result);
    }

    [Fact]
    public void Quote_WithInvalidIdentifier_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => PostgreSqlIdentifier.Quote("outbox.items"));

        Assert.Equal("value", exception.ParamName);
    }
}
