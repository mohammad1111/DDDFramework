using Gig.Framework.Core.Caching;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Exceptions;
using Gig.Framework.Core.RuleEngine;
using Gig.Framework.Core.Serilizer;
using Gig.Framework.Core.Settings;
using Gig.Framework.RuleEngine.Contract.CacheKeys;
using Gig.Framework.RuleEngine.Contract.Contracts;
using Gig.Framework.RuleEngine.Contract.Models;
using NRules;
using NRules.Fluent;
using Serilog;

namespace Gig.Framework.RuleEngine;

public class RuleEngine<TOut> : IRuleEngine<TOut> where TOut : GigRuleResultModel
{
    private readonly IDataSetting _dataSetting;
    private readonly IEventBus _eventBus;
    private readonly ILogger _logger;
    private readonly IRuleRepository _repository;
    private readonly IRequestMemoryCacheManager _requestMemoryCacheManager;
    private readonly ISerializer _serializer;
    private readonly IServiceLocator _serviceLocator;
    private Guid _recGuid;
    private Guid _ruleSetId = Guid.Empty;

    protected RuleEngine()
    {
    }

    public RuleEngine(IEventBus eventBus, IRuleRepository repository, ISerializer serializer, IDataSetting dataSetting,
        IRequestMemoryCacheManager requestMemoryCacheManager, ILogger logger, IServiceLocator serviceLocator)
    {
        _eventBus = eventBus;
        _repository = repository;
        _serializer = serializer;
        _dataSetting = dataSetting;
        _requestMemoryCacheManager = requestMemoryCacheManager;
        _logger = logger;
        _serviceLocator = serviceLocator;
    }

    public async Task<RunningRuleResult<TOut>> Run(GigRuleSetModel gigRuleSets)
    {
       
        _logger.Information("start RuleEngine with {GigRuleSets}", gigRuleSets);
        _ruleSetId = gigRuleSets.Id;
        _recGuid = gigRuleSets.RecGuid;
        if (!gigRuleSets.Rules.Any())
        {
            await SaveRuleSet();
            return await GetResult();
        }
        var repository = new RuleRepository
        {
            Activator = new GigRuleActivator(_serviceLocator.Current)
        };
        await _requestMemoryCacheManager.AddByExpireTimeAsync(new GigRulePriorityCacheKey { Id = 1 },
            gigRuleSets.Rules.OrderBy(x => x.Priority).Select(x => new RulePriority(x.RuleType, x.Priority)).ToList(),
            ExpirationMode.Absolute,
            TimeSpan.FromSeconds(180));
        var ruleTypes = gigRuleSets.Rules.OrderBy(x => x.Priority).Select(a => a.RuleType).Distinct().ToList();

        repository.Load(spec => { spec.From(ruleTypes); });
        var factory = repository.Compile();

        var session = factory.CreateSession();

        ((GigRuleModel)gigRuleSets.Model).ruleSetModel = gigRuleSets;

        await SaveRuleSet();

        var result = await ExecuteRuleAsync(session, gigRuleSets.Model);
        await HandleResult(result);
        _logger.Information("Response RuleEngine with {Result}", gigRuleSets);

        return result;
    }

    public async Task<bool> RollBack(Guid ruleSetId)
    {
        return await RollBackRules(ruleSetId, _eventBus, _serializer, _repository);
    }

    public async Task<IEnumerable<TOut>> Commit(Guid ruleSetId)
    {
        return await CommitRules(ruleSetId, null, _eventBus, _serializer, _repository);
    }


    public async Task<IEnumerable<TOut>> Commit(Guid ruleSetId, Guid ruleId)
    {
        return await CommitRules(ruleSetId, ruleId, _eventBus, _serializer, _repository);
    }

    public async Task<RunningRuleResult<TOut>> GetResult()
    {
        return await GetResult<TOut>(_ruleSetId, _recGuid, _serializer, _repository);
    }
    
    private static async Task<List<TOut>> GetResultRule<TOut>(IEnumerable<Guid> ruleSetIds, ISerializer serializer,
        IRuleRepository repository) where TOut : GigRuleResultModel
    {
        var rules = await repository.GetRulesResult(ruleSetIds);
        var results = new List<TOut>();
        foreach (var rule in rules)
        {
            var methodInfo = typeof(ISerializer).GetMethod("Deserialize")
                ?.MakeGenericMethod(Type.GetType(rule.TypeOfData)!);
            var result = (dynamic)methodInfo!.Invoke(serializer, new[] { rule.RuleContent });
            ((GigRuleResultModel)result)!.RuleSetId = rule.RuleSetId;
            results.Add(result);
        }

        return results;
    }
    
    public static async Task<RunningRuleResult<TOut>> GetResult<TOut>(IEnumerable<Guid> ruleSetIds,Guid? recGuid,
        ISerializer serializer, IRuleRepository repository) where TOut : GigRuleResultModel
    {
        var result = await GetResultRule<TOut>(ruleSetIds, serializer, repository);
        var runningResult = new RunningRuleResult<TOut> {
            Result = result,
            IsComplete =!result.Any() || result.All(x => x.IsRulePassed),
            WarningMessages = result.Where(x => !x.IsRulePassed && x.Deterrent == Deterrent.Warning)
                .Select(x => new RuleMessage
                {
                    Code = x.Code,
                    Message = x.Message,
                    RuleId = x.RuleId
                }).ToList(),
            ErrorMessages = result.Where(x => !x.IsRulePassed && x.Deterrent == Deterrent.Stop)
                .Select(x => new RuleMessage
                {
                    Code = x.Code,
                    Message = x.Message,
                    RuleId = x.RuleId
                }).ToList(),
            RuleSetId = result.Any()?result.First().RuleSetId:ruleSetIds.First(),
            RecGuid = Guid.Empty
        };

        return runningResult;
    }

    
    public static async Task<RunningRuleResult<TOut>> GetResult<TOut>(Guid ruleSetId, Guid? recGuid,
        ISerializer serializer, IRuleRepository repository) where TOut : GigRuleResultModel
    {
        return await GetResult<TOut>(new []{ruleSetId},recGuid, serializer, repository);
    }

    private async Task SaveRuleSet()
    {
        await _repository.SaveRuleSet(new RunningGigRuleSet
        {
            TypeOfData = $"{typeof(TOut).FullName}, {typeof(TOut).Assembly.FullName.Split(',').First()}",
            ExpireTime = DateTime.Now.Add(TimeSpan.Parse(_dataSetting.RuleExpireTime)),
            HandelRule = false,
            RuleSetId = _ruleSetId
        });
    }

    private static async Task<IEnumerable<TOut>> CommitRules(Guid ruleSetId, Guid? ruleId, IEventBus eventBus,
        ISerializer serializer, IRuleRepository repository)
    {
        var rules = await repository.GetRuleResult(ruleSetId);
        var result = new List<TOut>();

        if (rules != null && rules.Any())
        {
            foreach (var ruleResult in rules.Where(x => x.RuleId == ruleId || ruleId == null))
                if (ruleResult.RuleContent.StartsWith("["))
                    result.AddRange(serializer.Deserialize<List<TOut>>(ruleResult.RuleContent));
                else
                    result.Add(serializer.Deserialize<TOut>(ruleResult.RuleContent));

            await PublishCommitEvent(result, eventBus, repository);
        }

        return result;
    }

    public static async Task<bool> RollBackRules(Guid ruleSetId, IEventBus eventBus, ISerializer serializer,
        IRuleRepository repository)
    {
        var rules = await repository.GetRuleResult(ruleSetId);
        if (rules != null && rules.Any())
        {
            var result = new List<TOut>();
            foreach (var ruleResult in rules)
                result.AddRange(serializer.Deserialize<List<TOut>>(ruleResult.RuleContent));

            await PublishRollbackEvent(eventBus, result, repository);
            await repository.RemoveRuleSet(ruleSetId);

            return true;
        }

        return false;
    }

    private async Task<RunningRuleResult<TOut>> ExecuteRuleAsync(ISession session, object model)
    {
        try
        {
            session.Insert(model);
            var ruleResult = await GetResult();
            _logger.Information("RuleEngine success {RuleSetId},{Result}", _ruleSetId, ruleResult);
            return ruleResult;
        }

        catch (Exception e)
        {
            _logger.Error(e, "GigRuleEngine: {RuleSetId},{Error},{InnerException}", _ruleSetId, e, e.InnerException);
            RunningRuleResult<TOut> result;
            if (e.InnerException != null && e.InnerException.GetType() == typeof(StopRuleException))
            {
                result = await GetResult();
                var errorResult = ((StopRuleException)e.InnerException).RuleResult;
                result.ErrorMessages = new List<RuleMessage>
                {
                    new()
                    {
                        Code = errorResult.Code,
                        Message = errorResult.Message,
                        RuleId = errorResult.RuleId,
                        Exception = e
                    }
                };
                return result;
            }

            if (e.InnerException != null && e.InnerException.GetType() == typeof(GigBusinessException))
            {
                result = await GetResult();
                var errorResult = (GigBusinessException)e.InnerException;
                result.ErrorMessages = new List<RuleMessage>
                {
                    new()
                    {
                        Code = errorResult.Code,
                        Message = errorResult.Message,
                        RuleId = Guid.Empty,
                        Exception = e
                    }
                };
                return result;
            }

            result = await GetResult();
            result.ErrorMessages = new List<RuleMessage>
            {
                new()
                {
                    Code = "-1",
                    Message = "خطای غیره منتظره",
                    RuleId = Guid.Empty,
                    Exception = e
                }
            };
            await PublishRollbackEvent(_eventBus, result.Result, _repository);
            return result;
        }
    }

    private async Task HandleResult(RunningRuleResult<TOut> result)
    {
        var hasStopResult = result.Result.Any(x => !x.IsRulePassed && x.Deterrent == Deterrent.Stop);
        if (hasStopResult) await PublishRollbackEvent(_eventBus, result.Result, _repository);
        await AddHandleResultToCacheAsync(result);
    }

    private async Task AddHandleResultToCacheAsync(RunningRuleResult<TOut> result)
    {
        var results = new List<RunningRuleResult<TOut>>
        {
            result
        };
        if (_requestMemoryCacheManager.ExistsKey(new GigRuleEngineCacheKey()))
            results.AddRange(
                await _requestMemoryCacheManager.GetAsync<List<RunningRuleResult<TOut>>>(new GigRuleEngineCacheKey()));
        await _requestMemoryCacheManager.AddOrReplaceAsync(new GigRuleEngineCacheKey(), results);
    }

    private static async Task PublishRollbackEvent(IEventBus eventBus, IEnumerable<TOut> resultModels,
        IRuleRepository repository)
    {
        var models = resultModels.SelectMany(x => x.Events).ToList();
        foreach (var model in models.Where(x => x.RuleEventType == RuleEventType.Rollback))
        {
            await eventBus.PublishAsync(model.EngineEvent);
            await repository.RemoveRuleResult(model.RuleId);
        }
    }

    private static async Task PublishCommitEvent(IEnumerable<TOut> resultModels, IEventBus eventBus,
        IRuleRepository repository)
    {
        var models = resultModels.SelectMany(x => x.Events).ToList();
        foreach (var model in models.Where(x => x.RuleEventType == RuleEventType.Commit))
        {
            await eventBus.PublishAsync(model.EngineEvent);
            await repository.RemoveRuleResult(model.RuleId);
        }
    }
}

