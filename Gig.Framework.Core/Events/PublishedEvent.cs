namespace Gig.Framework.Core.Events;

public class PublishedEvent : PublishEvent
{
    public PublishedEvent(PublisherEvent publisher)
    {
        UpdateDateTime = DateTime.Now;
    }


    public DateTime UpdateDateTime { get; }
}