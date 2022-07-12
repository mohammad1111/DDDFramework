using Gig.Framework.Core.Events;
using Gig.Framework.EventBus.Contracts;
using MassTransit;

namespace Gig.Framework.EventBus.Bus;

public class GigEventBus : IGigEventBus
{
    private readonly IBus _bus;

    public GigEventBus(IBus bus)
    {
        _bus = bus;
    }

    public Task PublishAsync<T>(T message) where T : IEvent
    {
        return _bus.Publish(message);
    }
}