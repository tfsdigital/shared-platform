namespace Shared.Queries.Pagination;

public class PagedList<TItem>(IEnumerable<TItem>? items, long count, int pageNumber, int pageSize) : IPagedList<TItem>
    where TItem : class
{
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
    public long TotalPages { get; } = (long)Math.Ceiling(count / (double)pageSize);
    public long TotalRecords { get; } = count;
    public IEnumerable<TItem>? Items => items;
}
