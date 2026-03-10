using Microsoft.Extensions.DependencyInjection;

namespace Shared.Caching.Distributed.Extensions;

public static class CachingExtensions
{
    public static IServiceCollection AddDistributedCaching(this IServiceCollection services,
        string redisConnectionString)
    {
        services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
}
