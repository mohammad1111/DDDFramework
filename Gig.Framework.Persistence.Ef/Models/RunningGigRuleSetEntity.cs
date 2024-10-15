using System;

namespace Gig.Framework.Persistence.Ef.Models;

public class RunningGigRuleSetEntity
{
    public Guid RuleSetId { get; set; }

    public bool HandelRule { get; set; }
}