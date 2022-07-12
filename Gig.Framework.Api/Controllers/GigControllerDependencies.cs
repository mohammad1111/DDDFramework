using Gig.Framework.Core.Security;
using Gig.Framework.Core.UserContexts;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Gig.Framework.Api.Controllers;

public class GigControllerDependencies : IGigControllerDependencies
{
    public GigControllerDependencies(ILogger logger, IUserContextService userContextService,
        IHttpContextAccessor httpContext, ISecurityManager securityManager)
    {
        Logger = logger;
        UserContextService = userContextService;
        HttpContext = httpContext;
        SecurityManager = securityManager;
    }

    public ILogger Logger { get; }
    public IUserContextService UserContextService { get; }

    public IHttpContextAccessor HttpContext { get; }
    public ISecurityManager SecurityManager { get; }
}