using Gig.Framework.Core.Events;
using Gig.Framework.RuleEngine.Contract.Models;

namespace Gig.Framework.RuleEngine.Contract.Contracts;

public abstract class RuleEngineEvent : IIntegrationMessage
{
    public Guid Id { get; set; }

    private SelectableWarningModel Model { get; set; }
    public Guid CorrelationEventId { get; set; } = Guid.NewGuid();
    public long CompanyId { get; set; }
    public long UserId { get; set; }
    public long BranchId { get; set; }
    public int LangTypeCode { get; set; }
    public long SubSystemId { get; set; }
    public bool IsAdmin { get; set; }
}