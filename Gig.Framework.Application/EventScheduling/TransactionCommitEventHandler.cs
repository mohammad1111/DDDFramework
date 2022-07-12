using Gig.Framework.Core;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Serilizer;
using Serilog;

namespace Gig.Framework.Application.EventScheduling;

public class TransactionCommitEventHandler : IEventHandler<TransactionCommitEvent>
{
    private readonly IEventBus _eventBus;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger _logger;
    private readonly IRequestContext _requestContext;
    private readonly ISerializer _serializer;

    public TransactionCommitEventHandler(IEventBus eventBus, IEventRepository eventRepository,
        ISerializer serializer, IDistributeCacheManager cacheManager, ILogger logger, IRequestContext requestContext)
    {
        _eventBus = eventBus;
        _eventRepository = eventRepository;
        _serializer = serializer;
        _logger = logger;
        _requestContext = requestContext;
    }

    public async Task HandleAsync(TransactionCommitEvent eventToHandle)
    {
        var publisherEvent = await _eventRepository.GetEvents(eventToHandle.TransactionEventId);
        await EventPublisherJob.PublishEvents(publisherEvent, _eventBus, _eventRepository, _serializer, _logger,
            _requestContext);
    }
}