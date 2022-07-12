namespace Gig.Framework.Core.Events;

public class TransactionCommitEvent : ILocalMessage
{
    public Guid TransactionEventId { get; set; }
    public Guid CorrelationEventId { get; set; } = Guid.NewGuid();
    public long CompanyId { get; set; }
    public long UserId { get; set; }
    public long BranchId { get; set; }
    public int LangTypeCode { get; set; }
    public long SubSystemId { get; set; }
    public bool IsAdmin { get; set; }
}