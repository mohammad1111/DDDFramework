using System;
using System.Threading.Tasks;
using Gig.Framework.Core.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Gig.Sample.UI.Config
{
    public class GigScopeMiddleware
    {
        private readonly IGigContainer _container;
        private readonly RequestDelegate _next;

        public GigScopeMiddleware(RequestDelegate next, IGigContainer container)
        {
            _next = next;
            _container = container;

        }
        public async Task InvokeAsync(HttpContext context)
        {
            using var scope = _container.CreateScope();
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }


}