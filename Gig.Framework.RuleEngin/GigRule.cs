using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gig.Framework.Core;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.Helper;
using Gig.Framework.Core.RuleEngine;
using Gig.Framework.Core.Serilizer;
using Gig.Framework.RuleEngine.Contract.CacheKeys;
using Gig.Framework.RuleEngine.Contract.Contracts;
using Gig.Framework.RuleEngine.Contract.Models;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using Serilog;
using IRuleRepository = Gig.Framework.Core.RuleEngine.IRuleRepository;

namespace Gig.Framework.RuleEngine;

public abstract class GigRule<TModel, TResult> : Rule
    where TResult : GigRuleResultModel
    where TModel : GigRuleModel
{
    private readonly ILogger _logger;
    private readonly IRequestContext _requestContext;
    private readonly IRequestMemoryCacheManager _requestMemoryCache;
    private readonly IRuleRepository _ruleRepository;
    private readonly ISerializer _serializable;
    private TResult _result;

    private Guid _ruleSetId = Guid.Empty;

    public Dictionary<string, string> GigRuleProperties = new();

    public List<GigRuleResultModel> Results = new();


    public GigRule(IServiceLocator serviceLocator)
    {
        _logger = serviceLocator.Current.Resolve<ILogger>();
        _requestMemoryCache = serviceLocator.Current.Resolve<IRequestMemoryCacheManager>();
        _ruleRepository = serviceLocator.Current.Resolve<IRuleRepository>();
        _serializable = serviceLocator.Current.Resolve<ISerializer>();
        _requestContext = serviceLocator.Current.Resolve<IRequestContext>();
    }

    public Guid RuleId { get; set; } = Guid.NewGuid();


    public string Code { get; set; }


    private TModel Model { get; set; }


    public Deterrent Deterrent => (Deterrent)int.Parse(GigRuleProperties["Deterrent"]);


    public virtual string Message => string.Empty;


    public virtual WarningMode WarningMode => WarningMode.YesNo;


    private static T RunSynchronously<T>(Task<T> task)
    {
        return GigAsyncHelpers.RunSync(() => task);
    }


    private void SetPriority(Func<int> priorityFunc)
    {
        Priority(priorityFunc.Invoke());
    }

    public override void Define()
    {
        var rulePriority = GigAsyncHelpers
            .RunSync(async () =>
                await _requestMemoryCache.GetAsync<List<RulePriority>>(new GigRulePriorityCacheKey { Id = 1 }))
            .FirstOrDefault(x => x.Type == GetType())?.Priority ?? 0;
        Priority(rulePriority);
        When()
            .Match(() => Model)
            .Match<TModel>(model => Match(model));
        Then()
            .Do(ctx => RunActionWhenMatch(ctx));
    }


    private void SetProperties()
    {
        var rules = Model.ruleSetModel.Rules.First(x => x.RuleType == GetType());

        foreach (var gigProperty in rules.Properties) GigRuleProperties.Add(gigProperty.Name, gigProperty.Value);
    }


    private bool Match(TModel model)
    {
        _logger.Information("Run Rule {Rule} with {Model}", GetType().FullName, Model);
        Model = model;
        _ruleSetId = model.ruleSetModel.Id;
        SetProperties();
        GigAsyncHelpers.RunSync(() => GetRuleResult());
        Code = GigRuleProperties["Code"];
        if (RuleId == Guid.Empty) RuleId = Guid.NewGuid();

        var ruleResults = Results.ToList();
        try
        {
            var result = RunSynchronously(Validate(model));
            if (!result)
            {
                _result = Activator.CreateInstance<TResult>();

                _result.Deterrent = RunSynchronously(RunActionWhenNotMatch(model));

                var commitEvent = AddEventCommitWhenWarningDeterrent(model);

                if (_result.Deterrent == Deterrent.Warning)
                {
                    _result.WarningRuleModel = AddWarningRuleModel(model);
                    _result.Code = _result.WarningRuleModel.Code;
                    _result.RuleId = RuleId;
                    _result.Message = _result.WarningRuleModel.Message;
                    _result.BusinessRuleId = long.Parse(GigRuleProperties["BusinessRuleId"]);


                    if (_result.WarningRuleModel != null) _result.WarningRuleModel.Code = GigRuleProperties["Code"];

                    if (commitEvent != null)
                    {
                        commitEvent.Id = RuleId;
                        _result.AddEvent(commitEvent, RuleEventType.Commit, _requestContext);
                    }
                }

                _result.IsRulePassed = false;

                if (_result.Deterrent == Deterrent.Stop)
                {
                    var stopRuleException = new StopRuleException(_result, Message, RuleId, _ruleSetId, Code,
                        _ruleRepository,
                        _serializable, ruleResults, long.Parse(GigRuleProperties["BusinessRuleId"]));
                    _logger.Error(stopRuleException, "Stop Rule {RuleId} and {Code} with {Exception}", RuleId, Code,
                        stopRuleException);
                    throw stopRuleException;
                }

                ruleResults.Add(_result);
                GigAsyncHelpers.RunSync(() => SetRuleResults(ruleResults));
                GigAsyncHelpers.RunSync(() => _ruleRepository.AddRule(new RunningGigRuleResult
                {
                    RuleId = RuleId,
                    RuleSetId = _ruleSetId,
                    TypeOfData =
                        $"{typeof(TResult).FullName}, {typeof(TResult).Assembly.FullName.Split(',').First()}",
                    IsRemoved = false,
                    RuleContent = _serializable.Serialize(_result),
                    BusinessRuleId = long.Parse(GigRuleProperties["BusinessRuleId"])
                }));
                _logger.Information("Complete Rule {Rule} with {Result}", GetType().FullName, _result);

                return false;
            }

            _result = RunSynchronously(RunActionWhenMatch(model));
            _result.RuleId = RuleId;
            _result.BusinessRuleId = long.Parse(GigRuleProperties["BusinessRuleId"]);
            _result.Deterrent = Deterrent;
            var rollBackEvent = AddEventRollbackWhenMatch(model);
            if (rollBackEvent != null)
            {
                rollBackEvent.Id = RuleId;
                _result.AddEvent(rollBackEvent, RuleEventType.Rollback, _requestContext);
            }

            _result.Code = GigRuleProperties["Code"];
            _result.IsRulePassed = true;
            ruleResults.Add(_result);
            GigAsyncHelpers.RunSync(() => SetRuleResults(ruleResults));


            GigAsyncHelpers.RunSync(() => _ruleRepository.AddRule(new RunningGigRuleResult
            {
                RuleId = RuleId,
                RuleSetId = _ruleSetId,
                TypeOfData = $"{_result.GetType()}, {_result.GetType().Assembly.FullName.Split(",").First()}",
                IsRemoved = false,
                RuleContent = _serializable.Serialize(_result),
                BusinessRuleId = long.Parse(GigRuleProperties["BusinessRuleId"])
            }));
            _logger.Information("Complete Rule {Rule} with {Result}", GetType().FullName, _result);
        }
        catch (Exception e) when (e is not StopRuleException)
        {
            throw new StopRuleException(_result ?? new GigRuleResultModel(), "خطای غیره منتظره", RuleId, _ruleSetId,
                Code, _ruleRepository,
                _serializable, ruleResults, long.Parse(GigRuleProperties["BusinessRuleId"]));
        }

        return true;
    }

    private async Task SetRuleResults(IEnumerable<GigRuleResultModel> result)
    {
        await _requestMemoryCache.AddOrReplaceByExpireTimeAsync(new GigRuleResultKey { Id = _ruleSetId }, result,
            ExpirationMode.Sliding, TimeSpan.FromMinutes(3));
    }

    private async Task GetRuleResult()
    {
        if (await _requestMemoryCache.Exists(new GigRuleResultKey
            {
                Id = _ruleSetId
            }))
        {
            Results = await _requestMemoryCache.GetAsync<List<GigRuleResultModel>>(new GigRuleResultKey
            {
                Id = _ruleSetId
            });
        }

        else
        {
            Results = new List<GigRuleResultModel>();
            await _requestMemoryCache.AddOrReplaceByExpireTimeAsync(new GigRuleResultKey { Id = _ruleSetId }, Results,
                ExpirationMode.Sliding, TimeSpan.FromMinutes(3));
        }
    }

    protected virtual GigWarningRuleModel AddWarningRuleModel(TModel model)
    {
        return new GigWarningRuleModel
        {
            Code = GigRuleProperties["Code"],
            IsSelected = true,
            Message = Message,
            WarningMode = WarningMode.YesNo
        };
    }

    protected virtual RuleEngineEvent AddEventCommitWhenWarningDeterrent(TModel model)
    {
        return null;
    }

    protected virtual RuleEngineEvent AddEventRollbackWhenMatch(TModel model)
    {
        return null;
    }

    protected abstract Task<Deterrent> RunActionWhenNotMatch(TModel model);

    protected abstract Task<bool> Validate(TModel model);

    protected abstract Task<TResult> RunActionWhenMatch(TModel model);

    private void RunActionWhenMatch(IContext context)
    {
        context.Insert(_result);
    }
}