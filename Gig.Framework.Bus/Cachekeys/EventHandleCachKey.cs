using Gig.Framework.Core.Caching;

namespace Gig.Framework.Bus.Cachekeys;

public class EventHandleCacheKey : CacheKey
{
    public EventHandleCacheKey(Guid eventId, string type)
    {
        EventId = eventId;
        Type = type;
    }

    public Guid EventId { get; set; }

    public string Type { get; set; }
}