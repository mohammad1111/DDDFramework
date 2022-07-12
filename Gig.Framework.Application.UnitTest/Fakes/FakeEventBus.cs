using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.Events;

namespace Gig.Framework.Application.UnitTest.Fakes
{
    public class FakeEventBus : IEventBus
    {
        internal ConcurrentQueue<IIntegrationMessage> OutBoxMessages { get; private set; } = new ConcurrentQueue<IIntegrationMessage>();

        private readonly IEnterpriseServiceBus _enterpriseServiceBus;

        public FakeEventBus(IEnterpriseServiceBus enterpriseServiceBus)
        {

            EventHandlerSubscribers = new List<object>();

            this._enterpriseServiceBus = enterpriseServiceBus;

            Subscribe(new ActionHandler<TransactionCommitedEvent>(a =>
            {
                PublishExternalMessages();
            }));
        }

        private List<object> EventHandlerSubscribers { get; }


        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent
        {
            EventHandlerSubscribers.Add(eventHandler);
        }

        public void Subscribe<TEvent>(Action<TEvent> action) where TEvent : IEvent
        {
            EventHandlerSubscribers.Add(action);
        }

        public void Publish<TEvent>(TEvent eventToPublish) where TEvent : IEvent
        {
            if (EventHandlerSubscribers.Count > 0)
            {
                var eligibleSubscribers = GetEligibleSubscribers<TEvent>(eventToPublish);
                foreach (IEventHandler<TEvent> eventHandler in eligibleSubscribers)
                {
                    eventHandler.HandleAsync(eventToPublish);
                }
            }

            var actions = EventHandlerSubscribers
                .Where(e => e.GetType().GenericTypeArguments[0].Name == eventToPublish.GetType().Name)
                .ToList();

            actions.ForEach(s =>
            {
                var methodInfo = s.GetType().GetMethod("Invoke");
                if (methodInfo != null) methodInfo.Invoke(s, new Object[] { eventToPublish });
            });

            EventHandlerSubscribers.RemoveAll(x => x is Action<TEvent>);
            if (eventToPublish is IIntegrationMessage)
            {
                OutBoxMessages.Enqueue((IIntegrationMessage)eventToPublish);
            }
        }

        public async Task PublishAsync<TEvent>(TEvent eventToPublish) where TEvent : IEvent
        {
            var eligibleSubscribers1 = GetEligibleSubscribers<TEvent>(eventToPublish);

            foreach (IEventHandler<TEvent> eventHandler in eligibleSubscribers1)
            {
                await eventHandler.HandleAsync(eventToPublish);
            }

            if (eventToPublish is IIntegrationMessage)
            {
                OutBoxMessages.Enqueue((IIntegrationMessage)eventToPublish);
            }
        }

        private void PublishExternalMessages()
        {
            while (OutBoxMessages.Any())
            {
                //TODO : Handle Exception 
                OutBoxMessages.TryDequeue(out IIntegrationMessage message);
                _enterpriseServiceBus.Publish(message);
            }
        }

        private IEnumerable<IEventHandler<TEvent>> GetEligibleSubscribers<TEvent>(object eventToPublish) where TEvent : IEvent
        {
            var subscribers = EventHandlerSubscribers.Where(s => s is IEventHandler<TEvent>).OfType<IEventHandler<TEvent>>().ToList();
            var inlineActionDelegates = EventHandlerSubscribers.Where(s => s is Action<TEvent>).OfType<Action<TEvent>>().Select(p => new ActionHandler<TEvent>(p)).ToList();
            var allResolvedHandlers = ServiceLocator.Current.ResolveAll<IEventHandler<TEvent>>();

            subscribers.AddRange(inlineActionDelegates);
            subscribers.AddRange(allResolvedHandlers);

            return subscribers;
        }
    }
}