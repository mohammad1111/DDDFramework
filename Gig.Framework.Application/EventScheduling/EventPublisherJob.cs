using System.Diagnostics;
using Gig.Framework.Core;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Serilizer;
using Gig.Framework.Scheduling;
using Quartz;
using Serilog;

namespace Gig.Framework.Application.EventScheduling;

public class EventPublisherJob : GigJobBase
{
    private readonly IEventBus _eventBus;
    private readonly IEventRepository _eventRepository;
    private readonly ISerializer _serializer;

    public EventPublisherJob(IEventBus eventBus, IEventRepository eventRepository, ISerializer serializer,
        ILogger logger, IRequestContext requestContext) : base(logger, requestContext)
    {
        _eventBus = eventBus;
        _eventRepository = eventRepository;
        _serializer = serializer;
    }

    public static async Task PublishEvents(IEnumerable<PublisherEvent> savedEvents, IEventBus eventBus,
        IEventRepository eventRepository, ISerializer serializer, ILogger logger, IRequestContext requestContext)
    {
        foreach (var publisherEvent in savedEvents)
        {
            logger.Information("publish Event To Broker {TraceId} - EventType: {EventType} - Event: {@Event} ",
                requestContext.GetUserContext().TraceId,
                publisherEvent.EventType.ToString(), publisherEvent);
            await eventBus.PublishIntegrationMessageAsync(publisherEvent.Deserialize(serializer));
            await eventRepository.CompleteEvent(publisherEvent);
        }
    }

    protected override async Task Execute(IJobExecutionContext gigJobExecutionContext)
    {
        var savedEvents = await _eventRepository.GetEvents();
        Debug.WriteLine($"Start at {DateTime.Now} and Items:{savedEvents.Count}");
        await PublishEvents(savedEvents, _eventBus, _eventRepository, _serializer, Logger, RequestContext);
    }
}