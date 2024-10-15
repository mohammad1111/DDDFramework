using System;

namespace Gig.Framework.Persistence.Ef.Models;

public class HandleInboxEvent
{
    public Guid DomainEventId { get; set; }

    public string HandlerType { get; set; }

    public DateTime? UpdateDateTime { get; set; }
}