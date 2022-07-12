using System.Transactions;
using Gig.Framework.Core.Helper;
using Gig.Framework.Core.Serilizer;

namespace Gig.Framework.Core.Events;

public class PublisherEvent : PublishEvent
{
    private static readonly Type LocalMessageType = typeof(ILocalMessage);
    private static readonly Type IntegrationType = typeof(IIntegrationMessage);

    public PublisherEvent(Guid domainEventId, string entityName, string eventContent,
        string eventTypeString, DateTime createDateTime, Guid? transactionEventId)
    {
        DomainEventId = domainEventId;
        EntityName = entityName;
        EventContent = eventContent;
        EventTypeString = eventTypeString;
        CreateDateTime = createDateTime;
        TransactionEventId = transactionEventId;
        Type = Events.EventType.Integrate;
    }

    public PublisherEvent(IEvent eventContent, string eventTypeString, string entityName, ISerializer serializer)
    {
        var interfaces = eventContent.GetType().GetInterfaces();
        Object = eventContent;
        EventContent = serializer.Serialize(eventContent);
        EventTypeString = eventTypeString;
        EntityName = entityName;
        DomainEventId = Guid.NewGuid();
        Type = SetType(interfaces);
        TransactionEventId = Transaction.Current == null
            ? null
            : Transaction.Current.TransactionInformation.DistributedIdentifier;
    }

    public IEvent Object { get; set; }

    public Guid? TransactionEventId { get; set; }

    public EventType Type { get; set; }

    public DateTime CreateDateTime { get; set; }

    private EventType SetType(Type[] interfaces)
    {
        if (interfaces.Contains(LocalMessageType) && interfaces.Contains(IntegrationType)) return Events.EventType.Both;

        if (!interfaces.Contains(LocalMessageType) && !interfaces.Contains(IntegrationType))
            return Events.EventType.None;

        if (interfaces.Contains(IntegrationType) && !interfaces.Contains(LocalMessageType))
            return Events.EventType.Integrate;

        return Events.EventType.Local;
    }

    public dynamic Deserialize(ISerializer serializer)
    {
        var method = typeof(ISerializer).GetMethod(nameof(serializer.Deserialize));
        var generic = method?.MakeGenericMethod(EventType);
        return generic?.Invoke(serializer, new object[] { EventContent });
    }
}

public enum EventType
{
    None,
    Local,
    Integrate,
    Both
}