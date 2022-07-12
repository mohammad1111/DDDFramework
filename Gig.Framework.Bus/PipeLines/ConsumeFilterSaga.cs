using Gig.Framework.Bus.Cachekeys;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Exceptions;
using Gig.Framework.Core.UserContexts;
using GreenPipes;
using MassTransit;
using MassTransit.Internals.Extensions;
using MassTransit.Saga;
using Serilog;

namespace Gig.Framework.Bus.PipeLines;

public class ConsumeFilterSaga<T> : IFilter<SagaConsumeContext<T>>
    where T : class, ISaga
{
    private readonly IDistributeCacheManager _cacheManager;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger _logger;
    private readonly IUserContextService _userContextServiceService;


    public ConsumeFilterSaga(IUserContextService userContextServiceService, IDistributeCacheManager cacheManager,
        IEventRepository eventRepository, ILogger logger)
    {
        _userContextServiceService = userContextServiceService;
        _cacheManager = cacheManager;
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task Send(SagaConsumeContext<T> context, IPipe<SagaConsumeContext<T>> next)
    {
        ConsumeContext<IEvent> t = null;
        if (context.HasMessageType(typeof(IEvent)))
        {
            context.TryGetMessage(out t);
            _userContextServiceService.SetUserContext(t.Message);
        }

        if (t != null && await CanHandle(t.Message))
        {
            await next.Send(context);
            await SaveInInbox(t.Message);
        }
    }

    public void Probe(ProbeContext context)
    {
    }

    private async Task ValidateHandleCache(IEvent eventMessage)
    {
        FrameworkException exception = null;

        try
        {
            if (!_cacheManager.ExistsKey(new EventHandleCacheKey(eventMessage.CorrelationEventId,
                    GetType().FullName)))
                await _cacheManager.AddByExpireTimeAsync(new EventHandleCacheKey(
                        eventMessage.CorrelationEventId,
                        GetType().Name), eventMessage.CorrelationEventId, ExpirationMode.Absolute,
                    TimeSpan.FromSeconds(5));
            else
                exception = new FrameworkException("این درخواست در سیستم قبلا شروع به انجام شده است.");
        }
        catch (Exception e)
        {
            exception = new FrameworkException("این درخواست در سیستم قبلا شروع به انجام شده است.", e);
        }

        exception?.Rethrow();
    }


    private async Task<bool> CanHandle(IEvent @event)
    {
        //  await ValidateHandleCache(@event);
        if (await _eventRepository.IsHandelEvent(@event.CorrelationEventId, GetType().Name))
        {
            _logger.Error("This event with id({ID}) has already been handled", @event.CorrelationEventId);
            return false;
        }

        return true;
    }

    private async Task SaveInInbox(IEvent @event)
    {
        if (!await CanHandle(@event)) return;

        await _eventRepository.SaveInBoxEvent(@event.CorrelationEventId, GetType().Name);
    }
}