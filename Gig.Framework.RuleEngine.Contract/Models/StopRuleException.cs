using Gig.Framework.Core.Helper;
using Gig.Framework.Core.RuleEngine;
using Gig.Framework.Core.Serilizer;

namespace Gig.Framework.RuleEngine.Contract.Models;

public class StopRuleException : Exception
{
    public StopRuleException(GigRuleResultModel resultModel, string message, Guid ruleId, Guid ruleSetId, string code,
        IRuleRepository ruleRepository, ISerializer serializer, List<GigRuleResultModel> results, long businessRuleId)
    {
        resultModel.Deterrent = Deterrent.Stop;
        resultModel.Message = message;
        RuleResult = resultModel;
        results.Add(resultModel);
        resultModel.Code = code;
        GigAsyncHelpers.RunSync(() => ruleRepository.AddRule(new RunningGigRuleResult
        {
            RuleId = ruleId,
            RuleSetId = ruleSetId,
            TypeOfData =
                $"{typeof(GigRuleResultModel).FullName}, {typeof(GigRuleResultModel).Assembly.FullName.Split(',').First()}",
            IsRemoved = false,
            RuleContent = serializer.Serialize(resultModel),
            BusinessRuleId = businessRuleId
        }));
    }

    public GigRuleResultModel RuleResult { get; set; }
}