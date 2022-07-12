using CacheManager.Core;
using Gig.Framework.Core.Caching;

namespace Gig.Framework.Caching;

public class RequestMemoryCaching : CachingBase, IRequestMemoryCacheManager
{
    public RequestMemoryCaching() : base(CacheFactory.Build(settings => settings
        .WithUpdateMode(CacheUpdateMode.Up).WithDictionaryHandle()))
    {
    }
}