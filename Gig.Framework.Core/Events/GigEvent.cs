namespace Gig.Framework.Core.Events;

public abstract class GigEvent : IEvent
{
    protected GigEvent()
    {
    }

    protected GigEvent(IRequestContext context)
    {
        var user = context.GetUserContext();
        CorrelationEventId = Guid.NewGuid();
        CompanyId = user.CompanyId;
        UserId = user.UserId;
        BranchId = user.BranchId;
        LangTypeCode = user.LangTypeCode;
        SubSystemId = user.SubSystemId;
        IsAdmin = user.IsAdmin;
    }

    public Guid CorrelationEventId { get; set; }
    public long CompanyId { get; set; }
    public long UserId { get; set; }
    public long BranchId { get; set; }
    public int LangTypeCode { get; set; }
    public long SubSystemId { get; set; }
    public bool IsAdmin { get; set; }
}