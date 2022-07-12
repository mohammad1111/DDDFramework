namespace Gig.Framework.RuleEngine.Contract.Models;

public class RunningRuleResult<TOut>
{
    public IList<TOut> Result { get; set; }

    public bool IsComplete { get; set; }

    public IEnumerable<RuleMessage> WarningMessages { get; set; }

    public IEnumerable<RuleMessage> ErrorMessages { get; set; }


    public Guid RuleSetId { get; set; }

    public Guid RecGuid { get; set; }
}