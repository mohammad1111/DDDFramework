using System;
using System.Threading.Tasks;
using Autofac;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.UserContexts;
using MassTransit;

namespace Gig.Framework.Bus.PipeLines;

public class SecurityConsumeFilter : IFilter<ConsumeContext>
{
    private readonly ILifetimeScope _serviceProvider;

    public SecurityConsumeFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = (ILifetimeScope)serviceProvider.GetService(typeof(ILifetimeScope));
    }

    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
    {
        var cache = _serviceProvider.Resolve<IUserContextService>();
        try
        {
            if (context.HasMessageType(typeof(IEvent)))
            {
                context.TryGetMessage(out ConsumeContext<IEvent> t);
                await SendIEvent(t, next, cache);
                return;
            }

            await SendUnknownEvent(context, next, cache);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task SendIEvent(ConsumeContext<IEvent> context, IPipe<ConsumeContext> next, IUserContextService cache)
    {
        if (context.Message != null)
        {
            cache.SetUserContext(context.Message);
            await next.Send(context);
        }

        await next.Send(context);
    }

    private async Task SendUnknownEvent(ConsumeContext context, IPipe<ConsumeContext> next, IUserContextService cache)
    {
        await next.Send(context);
    }
}