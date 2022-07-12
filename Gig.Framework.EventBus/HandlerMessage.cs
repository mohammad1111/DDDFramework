using Gig.Framework.Core.Events;
using Gig.Framework.EventBus.Contracts;
using MassTransit;

namespace Gig.Framework.EventBus;

public abstract class GigHandlerMessage<T> : IConsumer<T> where T : class, IEvent
{
    // ReSharper disable once MemberCanBePrivate.Global
    public readonly IGigEventBus GigEventBus;

    protected GigHandlerMessage(IGigEventBus bus)
    {
        GigEventBus = bus;
    }

    public async Task Consume(ConsumeContext<T> context)
    {
        await Handler(context.Message);
    }

    protected abstract Task Handler(T message);
}