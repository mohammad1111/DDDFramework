using Gig.Framework.Core.UserContexts;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Gig.Framework.Api.Controllers;

public abstract class GigController : ControllerBase
{
    private readonly IUserContextService _userContextService;
    public readonly ILogger Logger;

    protected GigController(IGigControllerDependencies dependencies)
    {
        Logger = dependencies.Logger;
        _userContextService = dependencies.UserContextService;
        var context = dependencies.HttpContext.HttpContext;
        var accessToken = string.Empty;
        if (context.Request.Headers.Any(x => x.Key == "Authorization"))
            accessToken = context.Request.Headers["Authorization"].ToString()
                .Substring(context.Request.Headers["Authorization"].ToString().IndexOf(' ') + 1)
                .Trim();
        else if (context.Request.Query.Any(x => x.Key == "access_token"))
            accessToken = context.Request.Query["access_token"].ToString()
                .Substring(context.Request.Query["access_token"].ToString().IndexOf(' ') + 1)
                .Trim();

        _userContextService.SetUserContext(dependencies.SecurityManager.Validate(accessToken));
    }
}