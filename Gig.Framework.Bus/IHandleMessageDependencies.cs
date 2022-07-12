using Gig.Framework.Core;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Security;
using Gig.Framework.Core.UserContexts;
using Serilog;

namespace Gig.Framework.Bus;

public interface IHandleMessageDependencies
{
    IRequestContext RequestContext { get; }

    IRequestMemoryCacheManager CacheManager { get; }

    IEventRepository EventRepository { get; }

    ILogger Logger { get; }

    IUserContextService UserContextService { get; }

    ISecurityManager SecurityManager { get; }
}