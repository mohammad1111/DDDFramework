namespace Gig.Framework.Core.Events;

public class ActionHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
{
    private readonly Action<TEvent> _handler;

    public ActionHandler(Action<TEvent> handlerDelegate)
    {
        _handler = handlerDelegate;
    }


    public Task HandleAsync(TEvent eventToHandle)
    {
        return Task.Factory.StartNew(() => { _handler(eventToHandle); });
    }
}