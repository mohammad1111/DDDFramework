using MassTransit;

namespace Gig.Framework.Bus.GigRoutingSlip;

public abstract class GigRoutingSlipActivity<TArgument, TLog> : IActivity<TArgument, TLog>
    where TArgument : GigArgument
    where TLog : GigLog
{
    private readonly IGigRoutingSlipActivityDependencies _dependencies;

    protected GigRoutingSlipActivity(IGigRoutingSlipActivityDependencies dependencies)
    {
        _dependencies = dependencies;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<TArgument> context)
    {
        _dependencies.UserContextService.SetUserContext(context.Arguments);

        var domainEventId = context.Arguments.CorrelationEventId;
        var handlerType = $"{GetType()}-Execute";
        var isHandled = await _dependencies.EventRepository.IsHandelEvent(domainEventId, handlerType);
        if (isHandled)
        {
            _dependencies.Logger.Error(
                "This RoutingSlip Message already Handled EventId: {EventId}, HandlerType{EventType}", domainEventId,
                handlerType);
            return context.Terminate();
        }

        try
        {
            _dependencies.Logger.Information("Start Execute Activity ({Type}) With Argument {Argument}", GetType().Name,
                context.Arguments);

            var result = await ExecuteBusiness(context);
            var isFault = result.IsFaulted(out var exception);

            _dependencies.Logger.Information("End Execute Activity({Type})  With isFault: {IsFault} and {Exception} ",
                GetType().Name, isFault,
                exception?.ToString() ?? string.Empty);
            if (!isFault) await _dependencies.EventRepository.SaveInBoxEvent(domainEventId, handlerType);
            return result;
        }
        catch (Exception e)
        {
            _dependencies.Logger.Error("Execute Activity Raise Error {Error}", e.ToString());
            return context.Faulted();
        }
    }

    public async Task<CompensationResult> Compensate(CompensateContext<TLog> context)
    {
        _dependencies.UserContextService.SetUserContext(context.Log);

        var domainEventId = context.Log.CorrelationEventId;
        var handlerType = $"{GetType()}-Compensate";
        var isHandled = await _dependencies.EventRepository.IsHandelEvent(domainEventId, handlerType);
        if (isHandled)
        {
            _dependencies.Logger.Error(
                "This RoutingSlip Message already Handled EventId: {EventId}, HandlerType{EventType}", domainEventId,
                handlerType);
            return context.Failed();
        }

        _dependencies.Logger.Information("Start Compensate Activity({Type}) With Log {Log}", GetType().Name,
            context.Log);

        var result = await RollBack(context);

        var isFault = result.IsFailed(out var exception);
        _dependencies.Logger.Information("End Compensate Activity({Type}) With IsFailed: {IsFailed} and {Exception} ",
            GetType().Name,
            isFault, exception?.ToString() ?? string.Empty);
        if (!isFault) await _dependencies.EventRepository.SaveInBoxEvent(domainEventId, handlerType);
        return result;
    }

    protected abstract Task<ExecutionResult> ExecuteBusiness(ExecuteContext<TArgument> context);

    protected abstract Task<CompensationResult> RollBack(CompensateContext<TLog> context);
}