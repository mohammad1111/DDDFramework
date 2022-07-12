namespace Gig.Framework.Persistence.Ef.Models;

public class PublisherEventEntity
{
    public string EventContent { get; set; }

    public string EventTypeString { get; set; }

    public Guid DomainEventId { get; set; }

    public Guid? TransactionEventId { get; set; }

    public string UserContext { get; set; }

    public DateTime? CreateDateTime { get; set; }
}