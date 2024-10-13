using Gig.Framework.Core.Events;
using Gig.Framework.Core.Helper;
using MassTransit;

namespace Gig.Framework.Bus.PipeLines;

public class SecurityPublishFilter<T> : IFilter<PublishContext<T>> where T : class, IEvent
{
    private readonly ICrcEngine _crcEngine;

    public SecurityPublishFilter(ICrcEngine crcEngine)
    {
        _crcEngine = crcEngine;
    }

    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        var crc = _crcEngine.GenerateCheckSum(context.Message);
        context.Headers.Set("crc", crc);
        await next.Send(context);
    }
}