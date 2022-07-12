

// ReSharper disable once CheckNamespace
namespace Gig.Framework.Core.Events;

public interface IEvent
{
    long CompanyId { get; set; }

    long UserId { get; set; }

    long BranchId { get; set; }

    long LangTypeCode { get; set; }

    long SubSystemId { get; set; }

    bool IsAdmin { get; set; }

    Guid CorrelationEventId { get; set; }
}