namespace Gig.Framework.Bus.RequestClient;

public class RequestClientEvent
{
    private Guid CorrelationEventId { get; set; }

    private long CompanyId { get; set; }

    private long UserId { get; set; }

    private long BranchId { get; set; }

    private long LangTypeCode { get; set; }

    private long SubSystemId { get; set; }

    private bool IsAdmin { get; set; }
}