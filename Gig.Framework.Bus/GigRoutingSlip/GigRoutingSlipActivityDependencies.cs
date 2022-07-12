using Gig.Framework.Core;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Helper;
using Gig.Framework.Core.UserContexts;
using Serilog;

namespace Gig.Framework.Bus.GigRoutingSlip;

public class GigRoutingSlipActivityDependencies : IGigRoutingSlipActivityDependencies
{
    public GigRoutingSlipActivityDependencies(IUserContextService userContextService, ILogger logger,
        IEventRepository eventRepository, ICrcEngine crcEngine, IRequestContext requestContext, IEventBus eventBus)
    {
        UserContextService = userContextService;
        Logger = logger;
        EventRepository = eventRepository;
        CrcEngine = crcEngine;
        RequestContext = requestContext;
        EventBus = eventBus;
    }

    public IUserContextService UserContextService { get; }
    public IRequestContext RequestContext { get; }
    public ILogger Logger { get; }
    public ICrcEngine CrcEngine { get; }
    public IEventRepository EventRepository { get; }
    public IEventBus EventBus { get; }
}