using Gig.Framework.Core;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Security;
using Gig.Framework.Core.UserContexts;
using MassTransit.Scoping;
using Serilog;

namespace Gig.Framework.Bus;

public class HandleMessageDependencies : IHandleMessageDependencies
{
    public HandleMessageDependencies(IRequestMemoryCacheManager cacheManager, IEventRepository eventRepository,
        IUserContextService userContextService, IDistributeCacheManager distributeCacheManager, ILogger logger,
        ISecurityManager securityManager, IConsumerScopeProvider scopeProvider, IRequestContext requestContext)
    {
        CacheManager = cacheManager;
        EventRepository = eventRepository;
        UserContextService = userContextService;
        DistributeCacheManager = distributeCacheManager;
        Logger = logger;
        SecurityManager = securityManager;
        ScopeProvider = scopeProvider;
        RequestContext = requestContext;
    }

    public IConsumerScopeProvider ScopeProvider { get; }

    public IDistributeCacheManager DistributeCacheManager { get; }

    public IRequestContext RequestContext { get; }

    public IRequestMemoryCacheManager CacheManager { get; }

    public IEventRepository EventRepository { get; }

    public ILogger Logger { get; }

    public IUserContextService UserContextService { get; }
    public ISecurityManager SecurityManager { get; }
}