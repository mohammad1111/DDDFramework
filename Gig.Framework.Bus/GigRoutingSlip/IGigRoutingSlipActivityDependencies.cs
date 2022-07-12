using Gig.Framework.Core;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Helper;
using Gig.Framework.Core.UserContexts;
using Serilog;

namespace Gig.Framework.Bus.GigRoutingSlip;

public interface IGigRoutingSlipActivityDependencies
{
    IUserContextService UserContextService { get; }

    IRequestContext RequestContext { get; }

    ILogger Logger { get; }

    ICrcEngine CrcEngine { get; }

    IEventRepository EventRepository { get; }

    IEventBus EventBus { get; }
}