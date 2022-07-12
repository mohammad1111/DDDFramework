using CacheManager.Core;
using Gig.Framework.Core.Caching;

namespace Gig.Framework.Caching;

public class MemoryCaching : CachingBase, IMemoryCacheManager
{
    public MemoryCaching() : base(CacheFactory.Build(settings => settings
        .WithUpdateMode(CacheUpdateMode.Up).WithDictionaryHandle()))
    {
    }
}