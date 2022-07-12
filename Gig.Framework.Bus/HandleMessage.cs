using Gig.Framework.Core.Events;
using Gig.Framework.Core.Models;
using Gig.Framework.Core.Security;
using MassTransit;

namespace Gig.Framework.Bus;

public abstract class HandleMessage<TMessage> : IConsumer<TMessage> where TMessage : class, IEvent
{
    private readonly IHandleMessageDependencies _dependencies;

    protected HandleMessage(IHandleMessageDependencies dependencies)
    {
        _dependencies = dependencies;
    }

    public ConsumeContext<TMessage> ConsumeContext { get; private set; }


    public async Task Consume(ConsumeContext<TMessage> context)
    {
        try
        {
            _dependencies.UserContextService.SetUserContext(context.Message);
            var eventMessage = context.Message;

            if (!await CanHandel(eventMessage)) return;

            ConsumeContext = context;

            var value = new Tuple<Guid, string>(eventMessage.CorrelationEventId, GetType().ToString());
            await _dependencies.CacheManager.AddAsync(new InboxCacheKey(), value);

            await Handle(context.Message);
            await SaveInInBox(eventMessage);

            _dependencies.Logger.Information(
                "The Message Type:({Type}), Handled EventId:{EventId}  TraceId:{TraceId} in Time:{HandleTime}",
                GetType().Name, eventMessage.CorrelationEventId, _dependencies.RequestContext.GetUserContext().TraceId,
                DateTime.Now);
        }
        catch (Exception e)
        {
            _dependencies.Logger.Information("Handle Event {EventType} Error {Error}", GetType().ToString(),
                e.ToString());
            throw;
        }
    }

    private string GetValidToken(string token)
    {
        var request = _dependencies.SecurityManager.ValidateWithoutExpireTime(token);
        var model = new TokenClaimModel
        {
            UserId = request.UserId,
            LastName = request.DisplayName,
            CompanyId = request.CompanyId,
            BranchId = request.BranchId,
            LangTypeCode = 1,
            ListOfUserCompanies = request.UserCompanies,
            IsAdmin = request.IsAdmin
        };

        var options = new BearerTokenOptionsModel
        {
            Issuer = "Any",
            Audience = "Any",
            Key = "386B17FD36C9176795BF91ABEEC45",
            AccessTokenExpirationMinutes = 360,
            RefreshTokenExpirationMinutes = 360
        };

        return _dependencies.SecurityManager.GetAccessToken(model, options);
    }

    private async Task SaveInInBox(TMessage eventMessage)
    {
        if (eventMessage is IEvent domainEvent)
        {
            var eventId = domainEvent.CorrelationEventId;
            if (await CanHandel(eventMessage))
                await _dependencies.EventRepository.SaveInBoxEvent(eventId, GetType().ToString());
        }
    }

    private async Task<bool> CanHandel(TMessage eventMessage)
    {
        var type = GetType().ToString();
        if (eventMessage is IEvent domainEvent)
        {
            var eventId = domainEvent.CorrelationEventId;
            if (await _dependencies.EventRepository.IsHandelEvent(eventId, type))
            {
                _dependencies.Logger.Error(
                    "The Message With Type:({Type}) is already Handled EventId:{EventId}  TraceId:{TraceId}",
                    GetType().Name,
                    eventMessage.CorrelationEventId, _dependencies.RequestContext.GetUserContext().TraceId);
                return false;
            }
        }

        _dependencies.Logger.Information(
            "The Message Type:({Type}) Start Handling  EventId:{EventId}  TraceId:{TraceId}", GetType().Name,
            eventMessage.CorrelationEventId, _dependencies.RequestContext.GetUserContext().TraceId);
        return true;
    }

    protected abstract Task Handle(TMessage message);

    protected Task HandleResult(GigCommonResultBase result)
    {
        if (result.HasError) throw result.FriendlyMessages[0].Exception;
        return Task.CompletedTask;
    }
}