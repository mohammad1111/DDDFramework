using Gig.Framework.Core;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.UserContexts;
using Serilog;

namespace Gig.Framework.Bus.GigStateMachine;

public class GigActivityDependencies : IGigActivityDependencies
{
    public GigActivityDependencies(ILogger logger, IUserContextService userContextService,
        IRequestContext requestContext, IEventBus eventBus)
    {
        this.logger = logger;
        UserContextService = userContextService;
        RequestContext = requestContext;
        EventBus = eventBus;
    }

    public IRequestContext RequestContext { get; }
    public ILogger logger { get; }
    public IUserContextService UserContextService { get; }
    public IEventBus EventBus { get; }
}