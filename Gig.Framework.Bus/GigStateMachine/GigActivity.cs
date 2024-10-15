using System;
using System.Threading.Tasks;
using Automatonymous;
using Gig.Framework.Core.Events;
using MassTransit;

namespace Gig.Framework.Bus.GigStateMachine;

public abstract class GigActivity<TInstance, TData> : IStateMachineActivity<TInstance, TData>
    where TInstance : GigSagaStateMachineInstance
    where TData : GigEvent
{
    private readonly IGigActivityDependencies _dependencies;

    public GigActivity(IGigActivityDependencies dependencies)
    {
        _dependencies = dependencies;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope(GetType().Name);
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    //
    // public async Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
    // {
    //     _dependencies.UserContextService.SetUserContext(context.Data);
    //     _dependencies.logger.Information("Start Activity({ActivityName})", GetType().Name);
    //     context.Instance.SetUser(_dependencies.RequestContext);
    //     await ExecuteBusiness(context);
    //
    //     await next.Execute(context).ConfigureAwait(false);
    //     _dependencies.logger.Information("End Activity({ActivityName})", GetType().Name);
    // }


    // public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
    //     Behavior<TInstance, TData> next) where TException : Exception
    // {
    //     _dependencies.UserContextService.SetUserContext(context.Data);
    //
    //     _dependencies.logger.Information("Start Rollback Activity({ActivityName})", GetType().Name);
    //     Rollback(context);
    //     _dependencies.logger.Information("End Rollback Activity({ActivityName})", GetType().Name);
    //
    //     return next.Faulted(context);
    // }

    protected abstract Task ExecuteBusiness(BehaviorContext<TInstance, TData> context);

    protected virtual Task Rollback<TException>(BehaviorExceptionContext<TInstance, TData, TException> context)
        where TException : Exception
    {
        return Task.CompletedTask;
    }

    public async Task Execute(BehaviorContext<TInstance, TData> context, IBehavior<TInstance, TData> next)
    {
        _dependencies.UserContextService.SetUserContext(context.Data);
        _dependencies.logger.Information("Start Activity({ActivityName})", GetType().Name);
        context.Instance.SetUser(_dependencies.RequestContext);
        await ExecuteBusiness(context);

        await next.Execute(context).ConfigureAwait(false);
        _dependencies.logger.Information("End Activity({ActivityName})", GetType().Name);
    }

    public  Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, IBehavior<TInstance, TData> next) where TException : Exception
    {
        _dependencies.UserContextService.SetUserContext(context.Data);

        _dependencies.logger.Information("Start Rollback Activity({ActivityName})", GetType().Name);
        Rollback(context);
        _dependencies.logger.Information("End Rollback Activity({ActivityName})", GetType().Name);

        return next.Faulted(context);
    }
}