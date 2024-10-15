using System;
using Gig.Framework.Core.Caching;

namespace Gig.Framework.RuleEngine.Contract.CacheKeys;

public class GigPropertiesCacheKey : CacheKey
{
    public Guid Id { get; set; }
}