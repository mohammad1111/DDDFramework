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

public class RuleExpireJob : GigJobBase
{
    private readonly IEventBus _eventBus;
    private readonly IRuleRepository _repository;
    private readonly ISerializer _serializer;

    public RuleExpireJob(IEventBus eventBus, ISerializer serializer, ILogger logger, IRuleRepository repository,
        IRequestContext requestContext) : base(logger, requestContext)
    {
        _eventBus = eventBus;
        _serializer = serializer;
        _repository = repository;
    }

    protected override async Task Execute(IJobExecutionContext gigJobExecutionContext)
    {
        var expireRules = await _repository.GetExpireRule();
        foreach (var expireRule in expireRules)
            await RunRuleEngineRollBack(expireRule.RuleSetId, _eventBus, _repository, _serializer,
                Type.GetType(expireRule.TypeOfData));
    }

    private async Task RunRuleEngineRollBack(
        Guid ruleSetId,
        IEventBus eventBus,
        IRuleRepository ruleRepository,
        ISerializer serializer,
        Type genericType)
    {
        var typ = typeof(RuleEngine<>);
        var ruleEngine = typ.MakeGenericType(genericType).GetMethod("RollBackRules");
        var param = new List<object> { ruleSetId, eventBus, serializer, ruleRepository };
        await (Task<bool>)ruleEngine.Invoke(null, param.ToArray());
    }
}