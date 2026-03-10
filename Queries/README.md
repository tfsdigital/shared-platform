# Queries

Pagination and query models for list endpoints.

## Architecture

- `BaseQuery`: Page number, page size, order by (with defaults and limits)
- `IPagedList<T>`, `PagedList<T>`: Paginated result wrapper

## Main Abstractions

```csharp
public abstract record BaseQuery
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public string? OrderBy { get; }
    public int Cursor => (PageNumber - 1) * PageSize;
}

public interface IPagedList<out TItem>
{
    int PageNumber { get; }
    int PageSize { get; }
    long TotalPages { get; }
    long TotalRecords { get; }
    IEnumerable<TItem>? Items { get; }
}
```

## Usage Example

```csharp
public record FindProductsQuery(int? Page, int? PageSize, string? OrderBy)
    : BaseQuery(Page, PageSize, OrderBy), IQuery<IPagedList<ProductDto>>;

public class FindProductsQueryHandler : IQueryHandler<FindProductsQuery, IPagedList<ProductDto>>
{
    public async Task<Result<IPagedList<ProductDto>>> Handle(FindProductsQuery query, CancellationToken ct)
    {
        var items = await _repository.GetPagedAsync(query.Cursor, query.PageSize, query.OrderBy, ct);
        var total = await _repository.CountAsync(ct);
        var paged = new PagedList<ProductDto>(items, total, query.PageNumber, query.PageSize);
        return Result.Success<IPagedList<ProductDto>>(paged);
    }
}
```
