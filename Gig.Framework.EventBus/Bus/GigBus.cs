using Gig.Framework.EventBus.Contracts;
using MassTransit;

namespace Gig.Framework.EventBus.Bus;

public class GigBus : IGigBus
{
    private readonly IBusControl _bus;

    public GigBus(IBusControl bus)
    {
        _bus = bus;
    }

    public void Start()
    {
        _bus.Start();
    }

    
    public Task StartAsync()
    {
        return _bus.StartAsync();
    }
}