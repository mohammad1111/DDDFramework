using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gig.Framework.Core;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.RuleEngine;
using Gig.Framework.Core.Serilizer;
using Gig.Framework.Scheduling;
using Quartz;
using Serilog;

namespace Gig.Framework.RuleEngine.RuleScheduling;

public class RuleConfirmJob : GigJobBase
{
    private readonly IEventBus _eventBus;
    private readonly IRuleRepository _ruleRepository;
    private readonly ISerializer _serializer;

    public RuleConfirmJob(ILogger logger, IRuleRepository ruleRepository, IEventBus eventBus, ISerializer serializer,
        IRequestContext requestContext) : base(logger, requestContext)
    {
        _ruleRepository = ruleRepository;
        _eventBus = eventBus;
        _serializer = serializer;
    }

    protected override async Task Execute(IJobExecutionContext gigJobExecutionContext)
    {
        var confirmRules = await _ruleRepository.GetCommitRule();
        foreach (var rule in confirmRules)
        {
            await RunRuleEngineCommit(rule.RuleSetId, _eventBus, _ruleRepository, _serializer,
                Type.GetType(rule.TypeOfData));
            await _ruleRepository.RemoveRuleSet(rule.RuleSetId);
        }
    }

    private async Task RunRuleEngineCommit(Guid ruleSetId, IEventBus eventBus, IRuleRepository ruleRepository,
        ISerializer serializer, Type genericType)
    {
        var typ = typeof(RuleEngine<>);
        var ruleEngine = typ.MakeGenericType(genericType).GetMethod("CommitRules");
        var param = new List<object> { ruleSetId, eventBus, serializer, ruleRepository };
        await (Task<bool>)ruleEngine.Invoke(null, param.ToArray());
    }
}