using Newtonsoft.Json;

namespace Gig.Framework.Core.Caching;

public abstract class CacheKey
{
    public string GetKey()
    {
        return GetSerializeKey(this);
    }

    private static string GetSerializeKey<T1>(T1 key) where T1 : CacheKey
    {
        return JsonConvert.SerializeObject(new CacheKeyGenerator<T1>(key), Formatting.Indented);
    }
}