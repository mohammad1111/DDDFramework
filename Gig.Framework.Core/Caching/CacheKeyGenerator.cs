namespace Gig.Framework.Core.Caching;

public class CacheKeyGenerator<T> where T : CacheKey
{
    private T _key;

    public CacheKeyGenerator()
    {
    }

    public CacheKeyGenerator(T cacheKey)
    {
        Key = cacheKey;
    }

    public T Key
    {
        get => _key;
        set
        {
            _key = value;
            TypeOfT = value.GetType().ToString();
        }
    }


    public string TypeOfT { get; set; }
}