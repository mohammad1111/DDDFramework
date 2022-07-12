using Automatonymous;
using Gig.Framework.Core;
using Gig.Framework.Core.Events;
using MassTransit.Saga;

namespace Gig.Framework.Bus.GigStateMachine;

public class GigSagaStateMachineInstance : GigEvent, SagaStateMachineInstance, ISagaVersion
{
    protected GigSagaStateMachineInstance()
    {
    }

    public int Version { get; set; }
    public Guid CorrelationId { get; set; }

    public void SetUser(IRequestContext context)
    {
        var user = context.GetUserContext();
        BranchId = user.BranchId;
        CompanyId = user.CompanyId;
        UserId = user.UserId;
        SubSystemId = user.SubSystemId;
        LangTypeCode = user.LangTypeCode;
        IsAdmin = user.IsAdmin;
    }
}