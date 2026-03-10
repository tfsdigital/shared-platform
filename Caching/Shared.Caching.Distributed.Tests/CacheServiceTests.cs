using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;

namespace Shared.Caching.Distributed.Tests;

public class CacheServiceTests
{
    private readonly IDistributedCache _distributedCache;
    private readonly CacheService _cacheService;

    public CacheServiceTests()
    {
        _distributedCache = Substitute.For<IDistributedCache>();
        _cacheService = new CacheService(_distributedCache);
    }

    [Fact]
    public async Task GetAsync_WhenCacheReturnsNull_ReturnsDefault()
    {
        // Arrange
        var key = "test-key";
        _distributedCache.GetAsync(key, Arg.Any<CancellationToken>()).Returns(Task.FromResult<byte[]?>(null));

        // Act
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_WhenCacheReturnsData_ReturnsDeserializedObject()
    {
        // Arrange
        var key = "test-key";
        var testObject = "test-value";
        var serializedData = JsonSerializer.SerializeToUtf8Bytes(testObject);
        _distributedCache.GetAsync(key, Arg.Any<CancellationToken>()).Returns(Task.FromResult<byte[]?>(serializedData));

        // Act
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        Assert.Equal(testObject, result);
    }

    [Fact]
    public async Task SetAsync_WithoutExpiration_CallsCacheSetWithDefaultExpiration()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";

        // Act
        await _cacheService.SetAsync(key, value);

        // Assert
        await _distributedCache.Received(1).SetAsync(
            key,
            Arg.Any<byte[]>(),
            Arg.Is<DistributedCacheEntryOptions>(opts => opts.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(2)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetAsync_WithExpiration_CallsCacheSetWithCustomExpiration()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var expiration = TimeSpan.FromMinutes(10);

        // Act
        await _cacheService.SetAsync(key, value, expiration);

        // Assert
        await _distributedCache.Received(1).SetAsync(
            key,
            Arg.Any<byte[]>(),
            Arg.Is<DistributedCacheEntryOptions>(opts => opts.AbsoluteExpirationRelativeToNow == expiration),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAsync_CallsCacheRemove()
    {
        // Arrange
        var key = "test-key";

        // Act
        await _cacheService.RemoveAsync(key);

        // Assert
        await _distributedCache.Received(1).RemoveAsync(key, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetAsync_SerializesValueCorrectly()
    {
        // Arrange
        var key = "test-key";
        var value = new { Name = "Test", Value = 123 };

        // Act
        await _cacheService.SetAsync(key, value);

        // Assert
        await _distributedCache.Received(1).SetAsync(
            key,
            Arg.Any<byte[]>(),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
    }
}
