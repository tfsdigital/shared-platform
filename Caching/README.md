# Caching

Abstraction for caching with Redis distributed implementation.

## Architecture

- **Shared.Caching**: Interface `ICacheService` only
- **Shared.Caching.Distributed**: Redis implementation via `IDistributedCache`

## Main Abstractions

```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
```

## Usage Example

```csharp
// Registration (Infrastructure)
services.AddDistributedCaching(redisConnectionString);

// Injection
public class MyService(ICacheService cache)
{
    public async Task<Product?> GetProduct(Guid id, CancellationToken ct)
    {
        var key = $"product:{id}";
        var cached = await cache.GetAsync<Product>(key, ct);
        if (cached is not null)
            return cached;

        var product = await _repository.GetById(id);
        if (product is not null)
            await cache.SetAsync(key, product, TimeSpan.FromMinutes(5), ct);

        return product;
    }
}
```
