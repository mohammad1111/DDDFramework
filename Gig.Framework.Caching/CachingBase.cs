using CacheManager.Core;
using Gig.Framework.Core.Caching;
using ExpirationMode = Gig.Framework.Core.Caching.ExpirationMode;

namespace Gig.Framework.Caching;

public abstract class CachingBase : ICacheManager
{
    protected readonly ICacheManager<object> CacheManager;


    protected CachingBase(ICacheManager<object> cacheManager)
    {
        CacheManager = cacheManager;
    }

    public async Task AddByExpireTimeAsync(string key, string cacheItem, ExpirationMode expirationMode,
        TimeSpan expireTime)
    {
        var mode = (CacheManager.Core.ExpirationMode)(int)expirationMode;
        await Task.Factory.StartNew(() => CacheManager.Add(new CacheItem<object>(key, cacheItem, mode, expireTime)));
    }

    public async Task AddOrUpdateAsync<T>(object key, T cacheItem)
    {
        await Task.Factory.StartNew(() => CacheManager.AddOrUpdate(key.ToString(), cacheItem, v => cacheItem));
    }

    public virtual async Task AddAsync<T>(CacheKey key, T cacheItem)
    {
        await Task.Factory.StartNew(() => CacheManager.Add(new CacheItem<object>(key.GetKey(), cacheItem)));
    }

    public virtual async Task AddOrUpdateAsync<T>(CacheKey key, T cacheItem)
    {
        await Task.Factory.StartNew(() => CacheManager.AddOrUpdate(key.GetKey(), cacheItem, v => cacheItem));
    }

    public virtual async Task AddByExpireTimeAsync<T>(CacheKey key, T cacheItem, ExpirationMode expirationMode,
        TimeSpan expireTime)
    {
        var mode = (CacheManager.Core.ExpirationMode)(int)expirationMode;
        await Task.Factory.StartNew(() =>
            CacheManager.Add(new CacheItem<object>(key.GetKey(), cacheItem, mode, expireTime)));
    }

    public virtual async Task AddOrUpdateByExpireTimeAsync<T>(CacheKey key, T cacheItem, ExpirationMode expirationMode,
        TimeSpan expireTime)
    {
        var mode = (CacheManager.Core.ExpirationMode)(int)expirationMode;
        await Task.Factory.StartNew(() =>
            CacheManager.AddOrUpdate(new CacheItem<object>(key.GetKey(), cacheItem, mode, expireTime), v => cacheItem)
        );
    }

    public virtual async Task AddOrReplaceAsync<T>(CacheKey key, T cacheItem)
    {
        await Task.Factory.StartNew(() => CacheManager.Put(key.GetKey(), cacheItem));
    }

    public virtual async Task AddOrReplaceByExpireTimeAsync<T>(CacheKey key, T cacheItem, ExpirationMode expirationMode,
        TimeSpan expireTime)
    {
        var mode = (CacheManager.Core.ExpirationMode)(int)expirationMode;
        await Task.Factory.StartNew(() =>
            CacheManager.Put(new CacheItem<object>(key.GetKey(), cacheItem, mode, expireTime))
        );
    }

    public virtual async Task<bool> UpdateAsync<T>(CacheKey key, T value)
    {
        var result = false;
        await Task.Factory.StartNew(() =>
            result = CacheManager.TryUpdate(key.GetKey(), v => value, 100, out _)
        );
        return result;
    }

    public virtual async Task<bool> RemoveAsync(CacheKey key)
    {
        var result = false;
        await Task.Factory.StartNew(() =>
            result = CacheManager.Remove(key.GetKey())
        );
        return result;
    }

    public virtual async Task<T> GetAsync<T>(CacheKey key)
    {
        var result = await Task.Factory.StartNew(() => CacheManager.Get<T>(key.GetKey()));
        return result;
    }

    public virtual async Task<T> GetAsync<T>(string key)
    {
        var result = await Task.Factory.StartNew(() => CacheManager.Get<T>(key));
        return result;
    }


    public T Get<T>(CacheKey key)
    {
        var result = CacheManager.Get<T>(key.GetKey());
        return result;
    }

    public virtual T Get<T>(string key)
    {
        var result = CacheManager.Get<T>(key);
        return result;
    }

    public virtual async Task<bool> Exists(CacheKey key)
    {
        return await Task.Factory.StartNew(() =>
            CacheManager.Exists(key.GetKey())
        );
    }

    public bool ExistsKey(CacheKey key)
    {
        return CacheManager.Exists(key.GetKey());
    }

    public virtual async Task ClearAsync()
    {
        await Task.Factory.StartNew(() =>
            CacheManager.Clear()
        );
    }
}