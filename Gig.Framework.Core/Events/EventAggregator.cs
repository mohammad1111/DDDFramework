using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.Helper;

namespace Gig.Framework.Core.Events;

public class EventAggregator : IEventBus
{
    private readonly IEnterpriseServiceBus _enterpriseServiceBus;
    private readonly List<object> _eventHandlerSubscribers;
    private readonly IRequestContext _requestContext;
    private readonly IServiceLocator _serviceLocator;

    public EventAggregator(IEnterpriseServiceBus enterpriseServiceBus, IServiceLocator serviceLocator,
        IRequestContext requestContext)
    {
        _eventHandlerSubscribers = new List<object>();

        _enterpriseServiceBus = enterpriseServiceBus;
        _serviceLocator = serviceLocator;
        _requestContext = requestContext;
    }

    public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent
    {
        _eventHandlerSubscribers.Add(eventHandler);
    }

    public void Subscribe<TEvent>(Action<TEvent> action) where TEvent : IEvent
    {
        _eventHandlerSubscribers.Add(action);
    }

    public void Publish<TEvent>(TEvent eventToPublish) where TEvent : IEvent
    {
        GigAsyncHelpers.RunSync(() => PublishAsync(eventToPublish));
    }

    public async Task PublishAsync<TEvent>(TEvent eventToPublish) where TEvent : IEvent
    {
        if (eventToPublish is IIntegrationMessage integrationMessage) await PublishExternalMessages(integrationMessage);

        await HandleEvent(eventToPublish);
    }

    public async Task PublishIntegrationMessageAsync<T>(T eventToPublish) where T : IEvent
    {
        if (eventToPublish is IIntegrationMessage integrationMessage) await PublishExternalMessages(integrationMessage);
    }

    public async Task PublishLocalMessageAsync<T>(T eventToPublish) where T : IEvent
    {
        await HandleEvent(eventToPublish);
    }

    private void SetBaseEvent(IEvent eventToPublish)
    {
        if (eventToPublish.CompanyId == 0)
        {
            var user = _requestContext.GetUserContext();
            eventToPublish.BranchId = user.BranchId;
            eventToPublish.CompanyId = user.CompanyId;
            eventToPublish.UserId = user.UserId;
            eventToPublish.SubSystemId = user.SubSystemId;
            eventToPublish.LangTypeCode = user.LangTypeCode;
            eventToPublish.IsAdmin = user.IsAdmin;
        }
    }

    private async Task HandleEvent<TEvent>(TEvent eventToPublish) where TEvent : IEvent
    {
        SetBaseEvent(eventToPublish);

        var eligibleSubscribers = GetEligibleSubscribers<TEvent>(eventToPublish);

        foreach (var eventHandler in eligibleSubscribers) await eventHandler.HandleAsync(eventToPublish);
    }

    private async Task PublishExternalMessages(IIntegrationMessage message)
    {
        SetBaseEvent(message);

        await _enterpriseServiceBus.Publish(message);
    }


    private IEnumerable<IEventHandler<TEvent>> GetEligibleSubscribers<TEvent>(object eventToPublish)
        where TEvent : IEvent
    {
        var subscribers = _eventHandlerSubscribers.Where(s => s is IEventHandler<TEvent>)
            .OfType<IEventHandler<TEvent>>().ToList();
        var inlineActionDelegates = _eventHandlerSubscribers.Where(s => s is Action<TEvent>).OfType<Action<TEvent>>()
            .Select(p => new ActionHandler<TEvent>(p)).ToList();
        var allResolvedHandlers = _serviceLocator.Current.ResolveAll<IEventHandler<TEvent>>();

        subscribers.AddRange(inlineActionDelegates);
        subscribers.AddRange(allResolvedHandlers);

        return subscribers;
    }
}