namespace Shared.Queries.Pagination;

public interface IPagedList<out TItem>
    where TItem : class
{
    int PageNumber { get; }
    int PageSize { get; }
    long TotalPages { get; }
    long TotalRecords { get; }
    IEnumerable<TItem>? Items { get; }
}

