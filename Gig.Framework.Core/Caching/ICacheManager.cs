namespace Gig.Framework.Core.Caching;

public interface ICacheManager
{
    Task AddByExpireTimeAsync(string key, string cacheItem, ExpirationMode expirationMode, TimeSpan expireTime);

    Task AddOrUpdateAsync<T>(object key, T cacheItem);

    Task AddAsync<T>(CacheKey key, T cacheItem);

    Task AddOrUpdateAsync<T>(CacheKey key, T cacheItem);

    Task AddByExpireTimeAsync<T>(CacheKey key, T cacheItem, ExpirationMode expirationMode, TimeSpan expireTime);

    Task AddOrUpdateByExpireTimeAsync<T>(CacheKey key, T cacheItem, ExpirationMode expirationMode, TimeSpan expireTime);

    Task AddOrReplaceAsync<T>(CacheKey key, T cacheItem);

    Task AddOrReplaceByExpireTimeAsync<T>(CacheKey key, T cacheItem, ExpirationMode expirationMode,
        TimeSpan expireTime);

    Task<bool> UpdateAsync<T>(CacheKey key, T value);

    Task<bool> RemoveAsync(CacheKey key);

    Task<T> GetAsync<T>(CacheKey key);

    Task<T> GetAsync<T>(string key);

    T Get<T>(string key);

    Task<bool> Exists(CacheKey key);

    bool ExistsKey(CacheKey key);
    T Get<T>(CacheKey key);

    Task ClearAsync();
}