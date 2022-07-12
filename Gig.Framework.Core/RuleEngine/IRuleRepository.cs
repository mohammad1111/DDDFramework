namespace Gig.Framework.Core.RuleEngine;

public interface IRuleRepository
{
    Task SaveRuleSet(RunningGigRuleSet ruleSet);

    Task AddRule(RunningGigRuleResult ruleResult);

    Task<IList<RunningGigRuleResult>> GetRuleResult(Guid ruleSetId);

    Task<IList<RunningGigRuleResult>> GetRulesResult(IEnumerable<Guid> rulesSetId);

    Task<RunningGigRuleSet> GetRuleSet(Guid ruleSetId);

    Task RemoveRuleSet(Guid ruleSetId);

    Task RemoveRuleResult(Guid ruleId);

    Task<IEnumerable<RunningGigRuleSet>> GetExpireRule();

    Task<IEnumerable<RunningGigRuleSet>> GetCommitRule();
}