namespace Gig.Framework.RuleEngine.Contract.Models;

public abstract class GigRuleModel
{
    public Guid Id { get; set; }

    public GigRuleSetModel ruleSetModel { get; set; }
}