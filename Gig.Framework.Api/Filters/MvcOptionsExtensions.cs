using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Gig.Framework.Api.Filters;

public static class MvcOptionsExtensions
{
    public static void UseNotFoundInsteadOfNoContent(this MvcOptions mvcOptions)
    {
        mvcOptions.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
        mvcOptions.Filters.Add<NotFoundInsteadOfNullResultValueFilterAttribute>();
    }
}