using Gig.Framework.Core.Events;
using MassTransit;
using MassTransit.Courier;

namespace Gig.Framework.Bus.GigRoutingSlip;

public abstract class GigRoutingSlipExecute<TArgument> : IExecuteActivity<TArgument>
    where TArgument : class, IEvent
{
    private readonly IGigRoutingSlipActivityDependencies _dependencies;

    protected GigRoutingSlipExecute(IGigRoutingSlipActivityDependencies dependencies)
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
            // throw new Exception("This RoutingSlip Message already Handled");
            return context.Terminate();
        }

        _dependencies.Logger.Information("Start Execute ExecuteActivity{Type} With Argument {Argument}", GetType().Name,
            context.Arguments);
        var result = await ExecuteBusiness(context);
        var isFault = result.IsFaulted(out var exception);

        _dependencies.Logger.Information("End Execute ExecuteActivity{Type} With isFault: {IsFault} and {Exception} ",
            GetType().Name, isFault,
            exception?.ToString() ?? string.Empty);

        if (!isFault) await _dependencies.EventRepository.SaveInBoxEvent(domainEventId, handlerType);

        return result;
    }

    protected abstract Task<ExecutionResult> ExecuteBusiness(ExecuteContext<TArgument> context);
}