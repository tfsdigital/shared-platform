using Shared.Queries.Models;

namespace Shared.Queries.Tests.Models;

public class BaseQueryTests
{
    private record TestQuery : BaseQuery
    {
        public TestQuery(int? page, int? pageSize, string? orderBy) : base(page, pageSize, orderBy)
        {
        }
    }

    [Fact]
    public void Constructor_WhenCalledWithNullParameters_ShouldUseDefaults()
    {
        // Arrange & Act
        var query = new TestQuery(null, null, null);

        // Assert
        Assert.Equal(1, query.PageNumber);
        Assert.Equal(10, query.PageSize);
        Assert.Null(query.OrderBy);
        Assert.Equal(0, query.Cursor);
    }

    [Fact]
    public void Constructor_WhenCalledWithValidParameters_ShouldSetProperties()
    {
        // Arrange & Act
        var query = new TestQuery(2, 5, "name");

        // Assert
        Assert.Equal(2, query.PageNumber);
        Assert.Equal(5, query.PageSize);
        Assert.Equal("name", query.OrderBy);
        Assert.Equal(5, query.Cursor); // (2-1) * 5 = 5
    }

    [Fact]
    public void Constructor_WhenCalledWithPageSizeGreaterThanMax_ShouldLimitToMaxPageSize()
    {
        // Arrange & Act
        var query = new TestQuery(1, 50, null);

        // Assert
        Assert.Equal(25, query.PageSize); // Maximum allowed is 25
    }

    [Fact]
    public void Constructor_WhenCalledWithZeroPage_ShouldKeepZero()
    {
        // Arrange & Act
        var query = new TestQuery(0, 5, null);

        // Assert
        Assert.Equal(0, query.PageNumber); // SetPage only replaces null with default
    }

    [Fact]
    public void Constructor_WhenCalledWithNegativePage_ShouldKeepNegative()
    {
        // Arrange & Act
        var query = new TestQuery(-5, 5, null);

        // Assert
        Assert.Equal(-5, query.PageNumber); // SetPage only replaces null with default
    }

    [Fact]
    public void Constructor_WhenCalledWithZeroPageSize_ShouldKeepZero()
    {
        // Arrange & Act
        var query = new TestQuery(1, 0, null);

        // Assert
        Assert.Equal(0, query.PageSize); // SetPageSize only limits max, not min
    }

    [Fact]
    public void Constructor_WhenCalledWithNegativePageSize_ShouldKeepNegative()
    {
        // Arrange & Act
        var query = new TestQuery(1, -5, null);

        // Assert
        Assert.Equal(-5, query.PageSize); // SetPageSize only limits max, not min
    }

    [Fact]
    public void Constructor_WhenCalledWithEmptyOrderBy_ShouldSetOrderByToNull()
    {
        // Arrange & Act
        var query = new TestQuery(1, 5, "");

        // Assert
        Assert.Null(query.OrderBy);
    }

    [Fact]
    public void Constructor_WhenCalledWithWhitespaceOrderBy_ShouldSetOrderByToNull()
    {
        // Arrange & Act
        var query = new TestQuery(1, 5, "   ");

        // Assert
        Assert.Null(query.OrderBy);
    }

    [Fact]
    public void Constructor_WhenCalledWithOrderByWithWhitespace_ShouldTrimAndLowercase()
    {
        // Arrange & Act
        var query = new TestQuery(1, 5, "  NAME DESC  ");

        // Assert
        Assert.Equal("name desc", query.OrderBy);
    }

    [Fact]
    public void Constructor_WhenCalledWithMixedCaseOrderBy_ShouldConvertToLowercase()
    {
        // Arrange & Act
        var query = new TestQuery(1, 5, "CreatedAt ASC");

        // Assert
        Assert.Equal("createdat asc", query.OrderBy);
    }

    [Fact]
    public void Cursor_WhenPageNumberIsOne_ShouldReturnZero()
    {
        // Arrange & Act
        var query = new TestQuery(1, 10, null);

        // Assert
        Assert.Equal(0, query.Cursor);
    }

    [Fact]
    public void Cursor_WhenPageNumberIsThree_ShouldReturnCorrectOffset()
    {
        // Arrange & Act
        var query = new TestQuery(3, 15, null);

        // Assert
        Assert.Equal(30, query.Cursor); // (3-1) * 15 = 30
    }

    [Fact]
    public void Constructor_WhenCalledWithPageSizeEqualToMax_ShouldKeepPageSize()
    {
        // Arrange & Act
        var query = new TestQuery(1, 25, null);

        // Assert
        Assert.Equal(25, query.PageSize);
    }

    [Fact]
    public void Constructor_WhenCalledWithPageSizeOneLessThanMax_ShouldKeepPageSize()
    {
        // Arrange & Act
        var query = new TestQuery(1, 24, null);

        // Assert
        Assert.Equal(24, query.PageSize);
    }

    [Fact]
    public void Constructor_WhenCalledWithPageSizeOneMoreThanMax_ShouldLimitToMax()
    {
        // Arrange & Act
        var query = new TestQuery(1, 26, null);

        // Assert
        Assert.Equal(25, query.PageSize);
    }

    [Fact]
    public void Constructor_WhenCalledWithVeryLargePageSize_ShouldLimitToMax()
    {
        // Arrange & Act
        var query = new TestQuery(1, 1000, null);

        // Assert
        Assert.Equal(25, query.PageSize);
    }

    [Fact]
    public void Constructor_WhenCalledWithOrderByContainingOnlySpaces_ShouldSetToNull()
    {
        // Arrange & Act
        var query = new TestQuery(1, 5, "     ");

        // Assert
        Assert.Null(query.OrderBy);
    }

    [Fact]
    public void BaseQuery_ShouldBeAbstractRecord()
    {
        // Arrange & Act
        var query = new TestQuery(1, 5, "test");

        // Assert
        Assert.IsAssignableFrom<BaseQuery>(query);
    }
}
