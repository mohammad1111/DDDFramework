using Gig.Framework.Core.Events;
using GreenPipes;
using MassTransit.Courier;

namespace Gig.Framework.EventBus.Filters;

public class RoutingSlipCompensateFilter<T> : IFilter<CompensateContext<T>> where T : class, IEvent
{
    public Task Send(CompensateContext<T> context, IPipe<CompensateContext<T>> next)
    {
        return next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
    }
}