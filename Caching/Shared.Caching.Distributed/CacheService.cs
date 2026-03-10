using System.Buffers;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Shared.Caching.Distributed;

public class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var bytes = await cache.GetAsync(key, cancellationToken);

        return bytes is null ? default : Deserialize<T>(bytes);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var bytes = Serialize(value);

        return cache.SetAsync(key, bytes, CreateCacheOptions(expiration), cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        cache.RemoveAsync(key, cancellationToken);

    private static T Deserialize<T>(byte[] bytes) =>
        JsonSerializer.Deserialize<T>(bytes)!;

    private static readonly TimeSpan DefaultCacheExpiration = TimeSpan.FromMinutes(2);

    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, value);
        return buffer.WrittenSpan.ToArray();
    }

    private static DistributedCacheEntryOptions CreateCacheOptions(TimeSpan? expiration) =>
        new() { AbsoluteExpirationRelativeToNow = expiration ?? DefaultCacheExpiration };
}