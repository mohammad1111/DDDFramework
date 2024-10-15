using System;
using System.Collections.Generic;

namespace Gig.Framework.RuleEngine.Contract.Models;

public class GigRuleSetResultModel<TResult> where TResult : GigRuleResultModel
{
    public Guid Id { get; set; }

    public bool IsRulePassed { get; set; }

    public IEnumerable<TResult> Results { get; set; }
}