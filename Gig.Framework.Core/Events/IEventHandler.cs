namespace Gig.Framework.Core.Events;

public interface IEventHandler<T> where T : IEvent
{
    Task HandleAsync(T eventToHandle);
}