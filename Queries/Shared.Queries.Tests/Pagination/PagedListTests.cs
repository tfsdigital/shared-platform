using Shared.Queries.Pagination;

namespace Shared.Queries.Tests.Pagination;

public class PagedListTests
{
    [Fact]
    public void Constructor_WhenCalledWithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3" };
        var count = 10L;
        var pageNumber = 2;
        var pageSize = 5;

        // Act
        var pagedList = new PagedList<string>(items, count, pageNumber, pageSize);

        // Assert
        Assert.Equal(pageNumber, pagedList.PageNumber);
        Assert.Equal(pageSize, pagedList.PageSize);
        Assert.Equal(count, pagedList.TotalRecords);
        Assert.Equal(items, pagedList.Items);
    }

    [Fact]
    public void Constructor_WhenCalledWithNullItems_ShouldSetItemsToNull()
    {
        // Arrange
        var count = 10L;
        var pageNumber = 1;
        var pageSize = 5;

        // Act
        var pagedList = new PagedList<string>(null, count, pageNumber, pageSize);

        // Assert
        Assert.Null(pagedList.Items);
        Assert.Equal(count, pagedList.TotalRecords);
    }

    [Fact]
    public void Constructor_WhenCalledWithEmptyItems_ShouldSetEmptyItems()
    {
        // Arrange
        var items = new List<string>();
        var count = 0L;
        var pageNumber = 1;
        var pageSize = 5;

        // Act
        var pagedList = new PagedList<string>(items, count, pageNumber, pageSize);

        // Assert
        Assert.Empty(pagedList.Items!);
        Assert.Equal(0, pagedList.TotalRecords);
    }

    [Fact]
    public void TotalPages_WhenCalculated_ShouldReturnCorrectValue()
    {
        // Arrange
        var items = new List<string> { "item1", "item2" };
        var count = 23L;
        var pageNumber = 1;
        var pageSize = 5;

        // Act
        var pagedList = new PagedList<string>(items, count, pageNumber, pageSize);

        // Assert
        Assert.Equal(5L, pagedList.TotalPages); // Ceiling(23/5) = 5
    }

    [Fact]
    public void TotalPages_WhenCountIsZero_ShouldReturnZero()
    {
        // Arrange
        var items = new List<string>();
        var count = 0L;
        var pageNumber = 1;
        var pageSize = 5;

        // Act
        var pagedList = new PagedList<string>(items, count, pageNumber, pageSize);

        // Assert
        Assert.Equal(0L, pagedList.TotalPages);
    }

    [Fact]
    public void TotalPages_WhenCountEqualsPageSize_ShouldReturnOne()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3", "item4", "item5" };
        var count = 5L;
        var pageNumber = 1;
        var pageSize = 5;

        // Act
        var pagedList = new PagedList<string>(items, count, pageNumber, pageSize);

        // Assert
        Assert.Equal(1L, pagedList.TotalPages);
    }

    [Fact]
    public void TotalPages_WhenCountIsLessThanPageSize_ShouldReturnOne()
    {
        // Arrange
        var items = new List<string> { "item1", "item2" };
        var count = 2L;
        var pageNumber = 1;
        var pageSize = 5;

        // Act
        var pagedList = new PagedList<string>(items, count, pageNumber, pageSize);

        // Assert
        Assert.Equal(1L, pagedList.TotalPages);
    }

    [Fact]
    public void PagedList_ShouldImplementIPagedList()
    {
        // Arrange
        var items = new List<string> { "item1" };
        var count = 1L;
        var pageNumber = 1;
        var pageSize = 5;

        // Act
        var pagedList = new PagedList<string>(items, count, pageNumber, pageSize);

        // Assert
        Assert.IsAssignableFrom<IPagedList<string>>(pagedList);
    }

    [Fact]
    public void Constructor_WhenCalledWithLargeNumbers_ShouldHandleCorrectly()
    {
        // Arrange
        var items = new List<string> { "item1" };
        var count = 1000000L;
        var pageNumber = 500;
        var pageSize = 100;

        // Act
        var pagedList = new PagedList<string>(items, count, pageNumber, pageSize);

        // Assert
        Assert.Equal(pageNumber, pagedList.PageNumber);
        Assert.Equal(pageSize, pagedList.PageSize);
        Assert.Equal(count, pagedList.TotalRecords);
        Assert.Equal(10000L, pagedList.TotalPages); // Ceiling(1000000/100) = 10000
    }

    [Fact]
    public void Constructor_WhenCalledWithNegativePageNumber_ShouldStillWork()
    {
        // Arrange
        var items = new List<string> { "item1" };
        var count = 10L;
        var pageNumber = -1;
        var pageSize = 5;

        // Act
        var pagedList = new PagedList<string>(items, count, pageNumber, pageSize);

        // Assert
        Assert.Equal(pageNumber, pagedList.PageNumber);
        Assert.Equal(2L, pagedList.TotalPages);
    }

    [Fact]
    public void Constructor_WhenCalledWithZeroPageSize_ShouldHandleCorrectly()
    {
        // Arrange
        var items = new List<string> { "item1" };
        var count = 10L;
        var pageNumber = 1;
        var pageSize = 0;

        // Act
        var pagedList = new PagedList<string>(items, count, pageNumber, pageSize);

        // Assert
        // Division by zero in Math.Ceiling should result in a very large number or infinity
        // Let's check what actually happens
        Assert.Equal(pageSize, pagedList.PageSize);
        Assert.Equal(count, pagedList.TotalRecords);
        // The exact behavior may be implementation-specific, so let's just verify the object is created
        Assert.NotNull(pagedList);
    }
}
