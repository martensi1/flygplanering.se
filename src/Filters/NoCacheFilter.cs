using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace FlightPlanner.Service.Filters
{
    public class NoCacheFilter : IAsyncAlwaysRunResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var response = context.HttpContext.Response;

            response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
            response.Headers[HeaderNames.Expires] = "0";
            response.Headers[HeaderNames.Pragma] = "no-cache";

            await next.Invoke();
        }
    }
}
