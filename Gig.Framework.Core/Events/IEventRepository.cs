namespace Gig.Framework.Core.Events;

public interface IEventRepository
{
    Task<bool> IsHandelEvent(Guid eventId, string type);

    Task SaveInBoxEvent(Guid domainEventId, string type);

    Task CompleteEvent(PublisherEvent @event);

    Task<List<PublisherEvent>> GetEvents();

    Task<List<PublisherEvent>> GetEvents(Guid transactionId);

    Task<PublisherEvent> GetEvent(Guid domainEventId);
}