namespace Gig.Framework.Core.RuleEngine;

public class RunningGigRuleSet
{
    public Guid RuleSetId { get; set; }

    public DateTime ExpireTime { get; set; }

    public bool HandelRule { get; set; }

    public string TypeOfData { get; set; }

    public string MicroServiceName { get; set; }
}