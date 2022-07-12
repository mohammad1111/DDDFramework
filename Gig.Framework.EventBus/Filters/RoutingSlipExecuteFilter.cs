using Gig.Framework.Core.Events;
using GreenPipes;
using MassTransit.Courier;

namespace Gig.Framework.EventBus.Filters;

public class RoutingSlipExecuteFilter<T> : IFilter<ExecuteContext<T>> where T : class, IEvent
{
    public Task Send(ExecuteContext<T> context, IPipe<ExecuteContext<T>> next)
    {
        return next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
    }
}