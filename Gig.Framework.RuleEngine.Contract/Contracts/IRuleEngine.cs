using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gig.Framework.RuleEngine.Contract.Models;

namespace Gig.Framework.RuleEngine.Contract.Contracts;

public interface IRuleEngine<TOut>
{
    Task<RunningRuleResult<TOut>> Run(GigRuleSetModel gigRuleSets);

    Task<IEnumerable<TOut>> Commit(Guid ruleSetId, Guid ruleId);

    Task<IEnumerable<TOut>> Commit(Guid ruleSetId);

    Task<bool> RollBack(Guid ruleSetId);

    Task<RunningRuleResult<TOut>> GetResult();
}