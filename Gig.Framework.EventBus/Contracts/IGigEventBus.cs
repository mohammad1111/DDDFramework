using Gig.Framework.Core.Events;

namespace Gig.Framework.EventBus.Contracts;

public interface IGigEventBus
{
    Task PublishAsync<T>(T message) where T : IEvent;
}