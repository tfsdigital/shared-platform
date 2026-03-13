using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Caching;
using Shared.Caching.Distributed.Extensions;

namespace Shared.Caching.Distributed.Tests.Extensions;

public class CachingExtensionsTests
{
    [Fact]
    public void AddDistributedCaching_ShouldRegisterCacheServiceAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDistributedCaching("localhost:6379");

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ICacheService));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddDistributedCaching_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDistributedCaching("localhost:6379");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void AddDistributedCaching_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDistributedCaching("localhost:6379");

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddDistributedCaching_ShouldConfigureRedisConnectionString()
    {
        // Arrange
        var services = new ServiceCollection();
        var connectionString = "localhost:6379";

        services.AddDistributedCaching(connectionString);

        // Act
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<RedisCacheOptions>>();

        // Assert
        Assert.Equal(connectionString, options.Value.Configuration);
    }
}
