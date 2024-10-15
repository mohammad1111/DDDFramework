using System;

namespace Gig.Framework.Persistence.Ef.Models;

public class PublishedEventEntity
{
    public string EventContent { get; set; }

    public string EventTypeString { get; set; }

    public Guid DomainEventId { get; set; }

    public DateTime? UpdateDateTime { get; set; }
}