using Gig.Framework.Core.Caching;

namespace Gig.Framework.RuleEngine.Contract.CacheKeys;

public class GigRuleResultKey : CacheKey
{
    public Guid Id { get; set; }
}