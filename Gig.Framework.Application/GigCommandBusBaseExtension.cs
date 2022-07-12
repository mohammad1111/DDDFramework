using Gig.Framework.Core.Models;
using Gig.Framework.RuleEngine.Contract.Models;

namespace Gig.Framework.Application;

public static class GigCommandBusBaseExtension
{
    public static RunningRuleResult<GigRuleResultModel> GetRuleEngineResult(this GigCommonResultBase commonResultBase)
    {
        return (RunningRuleResult<GigRuleResultModel>)commonResultBase.RuleResult;
    }
}