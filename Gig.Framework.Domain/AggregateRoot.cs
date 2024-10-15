using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Gig.Framework.Core.Events;
using Gig.Framework.Domain.IdGenerators;

namespace Gig.Framework.Domain;

public abstract class AggregateRoot : Entity, IAggregateRoot
{
    protected AggregateRoot()
    {
    }

  

    protected AggregateRoot(IGigIdGenerator idGenerator) : base(idGenerator)
    {
    }

    [NotMapped] public List<DomainEvent> Changes { get; set; } = new();

    public void AddEvents([NotNull] IEnumerable<DomainEvent> events)
    {
        foreach (var @event in events)
        {

            AddEvent(@event);

        }
    }

    public void AddEvent(DomainEvent @event)
    {
        if (@event is not null)
        {
            Changes.Add(@event);
        }
    }


    public void AddUniqueEvent(DomainEvent @event)
    {
        var oldEvent = Changes.FirstOrDefault(x => x.GetType() == @event.GetType());
        if (oldEvent is not null)
        {
            Changes.Remove(oldEvent);
        }
        AddEvent(@event);
    }
}