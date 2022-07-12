using CacheManager.Core;
using CacheManager.Serialization.Json;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.Settings;

namespace Gig.Framework.Caching;

public class RedisCache : CachingBase, IDistributeCacheManager
{
    public RedisCache(IDataSetting setting) : base(CreateCacheManager(setting))
    {
    }

    private static ICacheManager<object> CreateCacheManager(IDataSetting setting)
    {
        var builder = new ConfigurationBuilder("myCache");
        builder
            .WithRedisCacheHandle("redis")
            .EnableStatistics();
        builder.WithRedisConfiguration("redis", config =>
        {
            config
                .WithAllowAdmin()
                .WithDatabase(0)
                .WithConnectionTimeout(5000)
                .EnableKeyspaceEvents()
                .WithEndpoint(setting.RedisConnection, int.Parse(setting.RedisPort))
                .WithPassword(setting.RedisPassword);
        });
        builder.WithSerializer(typeof(JsonCacheSerializer));
        return new BaseCacheManager<object>(builder.Build());
    }
}