namespace Gig.Framework.Core.Events;

public interface IEventBus
{
    void Subscribe<T>(IEventHandler<T> handler) where T : IEvent;

    void Subscribe<TEvent>(Action<TEvent> action) where TEvent : IEvent;

    void Publish<T>(T eventToPublish) where T : IEvent;

    Task PublishAsync<T>(T eventToPublish) where T : IEvent;

    Task PublishIntegrationMessageAsync<T>(T eventToPublish) where T : IEvent;

    Task PublishLocalMessageAsync<T>(T eventToPublish) where T : IEvent;
}