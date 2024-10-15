using System;
using System.Collections.Generic;
using Gig.Framework.Core;
using Gig.Framework.RuleEngine.Contract.Contracts;

namespace Gig.Framework.RuleEngine.Contract.Models;

public class GigRuleResultModel
{
    public Guid RuleSetId { get; set; }
    
    public Guid RuleId { get; set; }
    public long BusinessRuleId { get; set; }

    public List<RuleEvent> Events { get; } = new();

    public bool IsRulePassed { get; set; }

    public Deterrent Deterrent { get; set; }
    public GigWarningRuleModel WarningRuleModel { get; set; }

    public string Message { get; set; }

    public string Code { get; set; }

    public void AddEvent(RuleEngineEvent ruleEngineEvent, RuleEventType ruleEventType, IRequestContext requestContext)
    {
        var userContext = requestContext.GetUserContext();
        ruleEngineEvent.Id = Guid.NewGuid();
        ruleEngineEvent.BranchId = userContext.BranchId;
        ruleEngineEvent.CompanyId = userContext.CompanyId;
        ruleEngineEvent.SubSystemId = userContext.SubSystemId;
        ruleEngineEvent.LangTypeCode = userContext.LangTypeCode;
        ruleEngineEvent.UserId = userContext.BranchId;
        ruleEngineEvent.IsAdmin = userContext.IsAdmin;

        Events.Add(new RuleEvent
        {
            EngineEvent = ruleEngineEvent,
            RuleEventType = ruleEventType,
            RuleType = ruleEngineEvent.GetType().AssemblyQualifiedName,
            RuleId = RuleId
        });
    }
}