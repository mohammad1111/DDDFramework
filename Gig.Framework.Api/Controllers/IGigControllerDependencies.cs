using Gig.Framework.Core.Security;
using Gig.Framework.Core.UserContexts;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Gig.Framework.Api.Controllers;

public interface IGigControllerDependencies
{
    ILogger Logger { get; }
    IUserContextService UserContextService { get; }

    IHttpContextAccessor HttpContext { get; }

    ISecurityManager SecurityManager { get; }
}