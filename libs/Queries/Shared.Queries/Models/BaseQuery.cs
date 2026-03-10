namespace Shared.Queries.Models;

public abstract record BaseQuery
{
    // Constants

    private const int _defaultPage = 1;
    private const int _defaultPageSize = 10;
    private const int _maxPageSize = 25;

    protected BaseQuery(int? page, int? pageSize, string? orderBy)
    {
        PageNumber = SetPage(page);
        PageSize = SetPageSize(pageSize);
        OrderBy = SetOrder(orderBy);
    }

    // Properties

    public int PageNumber { get; }
    public int PageSize { get; }
    public string? OrderBy { get; }
    public int Cursor => (PageNumber - 1) * PageSize;

    // Private Methods

    private static int SetPage(int? page)
    {
        return page ?? _defaultPage;
    }

    private static string? SetOrder(string? orderBy)
    {
        return string.IsNullOrWhiteSpace(orderBy) ? null : orderBy.Trim().ToLowerInvariant();
    }

    private static int SetPageSize(int? value)
    {
        var pageSize = value ?? _defaultPageSize;

        return pageSize > _maxPageSize ? _maxPageSize : pageSize;
    }
}
