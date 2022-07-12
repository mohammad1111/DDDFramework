using Microsoft.AspNetCore.Builder;

namespace Gig.Sample.UI.Config
{
    public static class GigScopeMiddlewareExtensions
    {
        public static IApplicationBuilder UseGigScope(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GigScopeMiddleware>();
        }
    }
}