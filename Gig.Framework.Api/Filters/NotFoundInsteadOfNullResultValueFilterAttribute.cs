using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Gig.Framework.Api.Filters;

public class NotFoundInsteadOfNullResultValueFilterAttribute : Attribute,
    IAlwaysRunResultFilter
{
    public void OnResultExecuted(ResultExecutedContext context)
    {
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult { Value: null })
        {
            context.Result = new NotFoundResult();
            context.Result = new NotFoundObjectResult(
                new
                {
                    Message = $"Resource with uri {context.HttpContext.Request.Path} was not found."
                }
            );
            Log.Information(
                "Resource with uri {Message} was not found",
                context.HttpContext.Request.Path
            );
        }
    }
}