using Gig.Framework.Core;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.UserContexts;
using Serilog;

namespace Gig.Framework.Bus.GigStateMachine;

public interface IGigActivityDependencies
{
    IRequestContext RequestContext { get; }

    ILogger logger { get; }

    IUserContextService UserContextService { get; }

    IEventBus EventBus { get; }
}