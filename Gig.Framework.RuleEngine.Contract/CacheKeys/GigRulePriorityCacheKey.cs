using Gig.Framework.Core.Caching;

namespace Gig.Framework.RuleEngine.Contract.CacheKeys;

public class GigRulePriorityCacheKey : CacheKey
{
    public long Id { get; set; }
}