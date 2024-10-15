using System;
using System.Net;
using System.Threading.Tasks;
using Gig.Framework.Core;
using Gig.Framework.Core.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Gig.Framework.Security;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeAttribute : ActionFilterAttribute
{
    public AuthorizeAttribute(params string[] operations)
    {
        Operations = operations;
    }


    public string[] Operations { get; set; }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var serviceLocator =
            (IServiceLocator)context.HttpContext.RequestServices.GetRequiredService(typeof(IServiceLocator));
        var requestContext = serviceLocator.Current.TryResolve<IRequestContext>();
        var userContext = requestContext.GetUserContext();
        try
        {
            if (userContext != null)
                if (!await requestContext.HasPermission(Operations))
                {
                    context.HttpContext.Response.StatusCode = 401;
                    context.Result = new JsonResult(new
                    {
                        HttpStatusCode.Unauthorized,
                        error = $"There is no permissions in this resource {Environment.NewLine} {string.Join(", ", Operations)}",
                        friendlyMessage = "مجوز انجام این عملیات برای کاربری شما تعریف نشده است"
                    });
                }
        }
        catch (Exception)
        {
            context.HttpContext.Response.StatusCode = 401;
            context.Result = new JsonResult(new { HttpStatusCode.BadRequest, error = "user has no access", friendlyMessage = "متاسفانه مجوز دسترسی شما به سامانه یافت نشد" });
        }

        await base.OnActionExecutionAsync(context, next);
    }
}