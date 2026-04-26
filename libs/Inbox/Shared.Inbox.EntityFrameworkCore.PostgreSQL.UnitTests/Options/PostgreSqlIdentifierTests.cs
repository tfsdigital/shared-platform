using Shared.Inbox.EntityFrameworkCore.PostgreSQL.Options;

namespace Shared.Inbox.EntityFrameworkCore.PostgreSQL.UnitTests.Options;

public class PostgreSqlIdentifierTests
{
    [Theory]
    [InlineData("orders")]
    [InlineData("_orders")]
    [InlineData("orders_2026")]
    [InlineData("Orders")]
    public void IsValid_WithValidIdentifier_ReturnsTrue(string value)
    {
        Assert.True(PostgreSqlIdentifier.IsValid(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1orders")]
    [InlineData("orders-items")]
    [InlineData("orders.items")]
    [InlineData("orders items")]
    public void IsValid_WithInvalidIdentifier_ReturnsFalse(string? value)
    {
        Assert.False(PostgreSqlIdentifier.IsValid(value));
    }

    [Fact]
    public void Quote_WithValidIdentifier_ReturnsQuotedIdentifier()
    {
        var result = PostgreSqlIdentifier.Quote("orders_2026");

        Assert.Equal("\"orders_2026\"", result);
    }

    [Fact]
    public void Quote_WithInvalidIdentifier_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => PostgreSqlIdentifier.Quote("orders.items"));

        Assert.Equal("value", exception.ParamName);
    }
}
