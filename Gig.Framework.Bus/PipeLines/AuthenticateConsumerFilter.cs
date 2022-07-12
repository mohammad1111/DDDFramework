using Gig.Framework.Core.Events;
using Gig.Framework.Core.Helper;
using Gig.Framework.Core.UserContexts;
using GreenPipes;
using MassTransit;

namespace Gig.Framework.Bus.PipeLines;

public class AuthenticateConsumerFilter<T> : IFilter<ConsumeContext<T>>
    where T : class, IEvent
{
    private readonly ICrcEngine _crcEngine;
    private readonly IUserContextService _userContextServiceService;

    public AuthenticateConsumerFilter(IUserContextService userContextServiceService, ICrcEngine crcEngine)
    {
        _userContextServiceService = userContextServiceService;
        _crcEngine = crcEngine;
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var crc = context.Headers.Get<string>("crc");
        _crcEngine.Validate(context.Message, crc);
        _userContextServiceService.SetUserContext(context.Message);
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
    }
}