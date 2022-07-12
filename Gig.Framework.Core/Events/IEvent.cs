namespace Gig.Framework.Core.Events;

public interface IEvent
{
    Guid CorrelationEventId { get; set; }

    long CompanyId { get; set; }

    long UserId { get; set; }

    long BranchId { get; set; }

    int LangTypeCode { get; set; }

    long SubSystemId { get; set; }

    bool IsAdmin { get; set; }
}