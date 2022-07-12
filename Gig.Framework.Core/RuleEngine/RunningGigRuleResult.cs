namespace Gig.Framework.Core.RuleEngine;

public class RunningGigRuleResult
{
    public Guid RuleSetId { get; set; }

    public Guid RuleId { get; set; }

    public string RuleContent { get; set; }

    public string TypeOfData { get; set; }

    public bool IsRemoved { get; set; }

    public long BusinessRuleId { get; set; }
}