using Gig.Framework.Core.Events;

namespace Gig.Framework.Domain;

public interface IAggregateRoot
{
    List<DomainEvent> Changes { get; set; }

    void Lock();

    void Pend();

    void Active();

    void DeActive();
}