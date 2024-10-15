using System;
using Gig.Framework.Core.Caching;

namespace Gig.Framework.RuleEngine.Contract.CacheKeys;

public class GigRulePersistKey : CacheKey
{
    public Guid Id { get; set; }
}