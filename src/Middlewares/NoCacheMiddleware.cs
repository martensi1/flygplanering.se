using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace FlightPlanner.Service.Middlewares
{
    public class NoCacheMiddleware
    {
        private readonly RequestDelegate _next;

        public NoCacheMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var response = context.Response;

            response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
            response.Headers[HeaderNames.Expires] = "0";
            response.Headers[HeaderNames.Pragma] = "no-cache";

            await _next.Invoke(context);
        }
    }

    public static class NoCacheMiddlewareExtensions
    {
        public static IApplicationBuilder UseNoResponseCaching(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NoCacheMiddleware>();
        }
    }
}
