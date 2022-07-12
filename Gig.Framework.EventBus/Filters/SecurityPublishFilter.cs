using Gig.Framework.Core.Events;
using Gig.Framework.EventBus.Contracts;
using GreenPipes;
using MassTransit;
using Newtonsoft.Json;

namespace Gig.Framework.EventBus.Filters;

public class SecurityPublishFilter<T> : IFilter<PublishContext<T>> where T : class, IEvent
{
    private readonly ICrcEngine _crcEngine;
    private readonly IServiceProvider _serviceProvider;

    public SecurityPublishFilter(IServiceProvider serviceProvider, ICrcEngine crcEngine)
    {
        _serviceProvider = serviceProvider;
        _crcEngine = crcEngine;
    }

    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        var crc = _crcEngine.GenerateCheckSum(JsonConvert.SerializeObject(context.Message));
        context.Headers.Set("crc", crc);

        await next.Send(context);
    }
}