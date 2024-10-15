using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Gig.Framework.Application.EventScheduling;
using Gig.Framework.Core;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Models;
using Gig.Framework.Core.RuleEngine;
using Gig.Framework.Core.Serilizer;
using Gig.Framework.Domain;
using Gig.Framework.RuleEngine.Contract.CacheKeys;
using Gig.Framework.RuleEngine.Contract.Models;
using Serilog;

namespace Gig.Framework.Application;

public class CommandBus : ICommandBus
{
    private readonly IEventBus _eventBus;
    private readonly IEventRepository _eventRepository;
    private readonly ICommandHandlerFactory _factory;
    private readonly ILogger _logger;
    private readonly IRequestContext _requestContext;
    private readonly IRequestMemoryCacheManager _requestMemoryCacheManager;
    private readonly IRuleRepository _ruleRepository;
    private readonly ISerializer _serializer;

    private List<RunningRuleResult<GigRuleResultModel>> _runningRuleResult;

    public CommandBus(ICommandHandlerFactory factory, IEventBus eventBus, ILogger logger,
        IEventRepository eventRepository, ISerializer serializer, IRuleRepository ruleRepository,
        IRequestMemoryCacheManager requestMemoryCacheManager, IRequestContext requestContext)
    {
        _factory = factory;
        _eventBus = eventBus;
        _logger = logger;
        _eventRepository = eventRepository;
        _serializer = serializer;
        _ruleRepository = ruleRepository;
        _requestMemoryCacheManager = requestMemoryCacheManager;
        _requestContext = requestContext;
    }

    public async Task<GigCommonResultBase> DispatchAsync<T>(T command) where T : ICommand
    {
        var traceId = _requestContext.GetUserContext().TraceId;
        _logger.Information(
            "Start Handle Command TraceId:{TraceId} CommandType:{CommandType}: CommandValue:{CommandValue}", traceId,
            typeof(T),
            _serializer.Serialize(command));
        var handler = _factory.CreateHandler<T>();
        try
        {
            await handler.HandleAsync(command);
            await GetRuleResult();
            var events = (await handler.UnitOfWorkWorkProvider.GetUnitOfWork().Commit(_runningRuleResult?.Select(x =>
                new GigEntityRulesResult
                {
                    RecGuid = x.RecGuid,
                    RuleSetId = x.RuleSetId
                }).ToList())).ToList();
            events.AddRange(handler.Events.Select(x=> new PublisherEvent(x,x.GetType().AssemblyQualifiedName,"",_serializer )));
            _logger.Information(
                "End Handle Command TraceId:{TraceId} CommandType:{CommandType}: CommandValue:{CommandValue}",
                traceId, typeof(T),
                _serializer.Serialize(command));

            await Publish(events.ToList(), traceId);
            return new GigCommonResultBase
            {
                HasError = false
            };
        }
        catch (RuleWarningException ex)
        {
            handler.UnitOfWorkWorkProvider.GetUnitOfWork().Rollback();
            return CreateMessage(ex.Message, ex.HResult.ToString(), ex, traceId);
        }
        catch (RuleException ex)
        {
            handler.UnitOfWorkWorkProvider.GetUnitOfWork().Rollback();
            await RollBackRuleEvent(traceId);
            return CreateMessage(ex.Message, ex.HResult.ToString(), ex, traceId);
        }
        catch (DomainException ex)
        {
            handler.UnitOfWorkWorkProvider.GetUnitOfWork().Rollback();
            await RollBackRuleEvent(traceId);
            return CreateMessage(ex.Message, ex.HResult.ToString(), ex, traceId);
        }
        catch (Exception ex)
        {
            handler.UnitOfWorkWorkProvider.GetUnitOfWork().Rollback();
            await RollBackRuleEvent(traceId);
            if (ex.InnerException != null && ex.InnerException is DomainException)
                return CreateMessage(ex.InnerException.Message, ex.HResult.ToString(), ex, traceId);
            return CreateMessage("متاسفانه در انجام کار، خطای واقع گردیده است.", "230", ex, traceId);
        }
    }

    public async Task<GigCommonResult<TResult>> DispatchAsync<T, TResult>(T command) where T : ICommand
    {
        var handler = _factory.CreateHandler<T, TResult>();
        try
        {
            var result = await handler.HandleAsync(command);
            await handler.UnitOfWorkWorkProvider.GetUnitOfWork().Commit();
            return new GigCommonResult<TResult>
            {
                Data = result,
                HasError = false
            };
        }
        catch (DomainException ex)
        {
            handler.UnitOfWorkWorkProvider.GetUnitOfWork().Rollback();
            return new GigCommonResult<TResult>
            {
                HasError = true,
                FriendlyMessages = new List<BusinessErrorMessage>
                {
                    new()
                    {
                        Message = ex.Message,
                        ErrorCode = ex.HResult.ToString(),
                        Exception = ex
                    }
                }
            };
        }
        catch (Exception ex)
        {
            handler.UnitOfWorkWorkProvider.GetUnitOfWork().Rollback();
            throw ex;
        }
    }


    private async Task<IList<PublisherEvent>> GetRollbackRuleEvents(
        IList<Guid> ruleSetIds)
    {
        var user = _requestContext.GetUserContext();
        var rollbackEvents = new List<PublisherEvent>();

        if (ruleSetIds.Any())
        {
            var results = (await _ruleRepository.GetRulesResult(ruleSetIds)).ToList();
            foreach (var rule in results)
            {
                var methodInfo = typeof(ISerializer).GetMethod("Deserialize")
                    ?.MakeGenericMethod(Type.GetType(rule.TypeOfData));
                var result = (GigRuleResultModel)methodInfo.Invoke(_serializer,
                    new[] { rule.RuleContent });

                foreach (var resultEvent in result.Events)
                {
                    resultEvent.EngineEvent.BranchId = user.BranchId;
                    resultEvent.EngineEvent.CompanyId = user.CompanyId;
                    resultEvent.EngineEvent.UserId = user.UserId;
                    resultEvent.EngineEvent.SubSystemId = user.SubSystemId;
                    resultEvent.EngineEvent.LangTypeCode = user.LangTypeCode;
                    resultEvent.EngineEvent.IsAdmin = user.IsAdmin;
                }

                Guid? nullGuid = null;
                rollbackEvents.AddRange(result.Events.Where(x => x.RuleEventType == RuleEventType.Rollback)
                    .Select(x =>
                        new PublisherEvent(result.RuleId, rule.TypeOfData,
                            _serializer.Serialize(x.EngineEvent),
                            x.RuleType, DateTime.Now,
                            Transaction.Current != null
                                ? Transaction.Current.TransactionInformation.DistributedIdentifier
                                : nullGuid)));
            }
        }

        return rollbackEvents;
    }

    private async Task RollBackRuleEvent(Guid tracId)
    {
        await GetRuleResult();
        if (_runningRuleResult == null) return;

        var ruleSetIds = _runningRuleResult.Select(x => x.RuleSetId).ToList();
        if (!ruleSetIds.Any()) return;
        var rollBackEvents = await GetRollbackRuleEvents(ruleSetIds);
        if (rollBackEvents.Any()) await Publish(rollBackEvents, tracId);
    }

    private static bool RunAmbientTransaction()
    {
        return Transaction.Current != null;
    }

    private async Task GetRuleResult()
    {
        if (await _requestMemoryCacheManager.Exists(new GigRuleEngineCacheKey()))
            _runningRuleResult =
                await _requestMemoryCacheManager.GetAsync<List<RunningRuleResult<GigRuleResultModel>>>(
                    new GigRuleEngineCacheKey());
    }

    private async Task Publish(IList<PublisherEvent> events, Guid tracId)
    {
        foreach (var publisherEvent in events.Where(x => x.Type == EventType.Local || x.Type == EventType.Both))
        {
            await _eventBus.PublishLocalMessageAsync((dynamic)publisherEvent.Object);
            _logger.Information("publish localEvent {TraceId} - EventType: {EventType} - {@Event} ", tracId,
                publisherEvent.EventType.ToString(), publisherEvent);
        }

        try
        {
            if (!RunAmbientTransaction())
            {
                var integrationEvents =
                    events.Where(x => x.Type == EventType.Integrate || x.Type == EventType.Both);
                await EventPublisherJob.PublishEvents(integrationEvents, _eventBus, _eventRepository, _serializer,
                    _logger, _requestContext);
            }
        }
        catch (Exception exp)
        {
            _logger.Error(exp, "failed in publish Integration Event TraceId:{TraceId}", tracId);
        }
    }

    private GigCommonResultBase CreateMessage(string message, string errorCode, Exception ex, Guid tracId)
    {
        var result = new GigCommonResultBase
        {
            HasError = true,
            FriendlyMessages = new List<BusinessErrorMessage>
            {
                new()
                {
                    Message = message,
                    ErrorCode = errorCode,
                    Exception = ex
                }
            }
        };
        _logger.Information("Result from TraceId:{TraceId} ReturnResult:{ReturnResult}", tracId, result);

        return result;
    }
}