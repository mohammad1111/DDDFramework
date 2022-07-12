using Gig.Framework.Core.Events;
using Gig.Framework.EventBus.Contracts;
using GreenPipes;
using MassTransit;
using Newtonsoft.Json;

namespace Gig.Framework.EventBus.Filters;

public class ConsumeContextFilter<T> : IFilter<ConsumeContext<T>> where T : class, IEvent
{
    private readonly ICrcEngine _crcEngine;
    private readonly IServiceProvider _serviceProvider;

    public ConsumeContextFilter(IServiceProvider serviceProvider, ICrcEngine crcEngine)
    {
        _serviceProvider = serviceProvider;
        _crcEngine = crcEngine;
    }

    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var crc = context.Headers.Get<string>("crc");
        var message = JsonConvert.SerializeObject(context.Message);
        if (!_crcEngine.Validate(message, crc)) throw new Exception("message");
        await next.Send(context);
    }
}