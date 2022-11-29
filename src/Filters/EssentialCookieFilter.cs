using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace FlightPlanner.Service.Filters
{
    public class EssentialCookieFilter : IAsyncPageFilter
    {
        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            await Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(
            PageHandlerExecutingContext context,
            PageHandlerExecutionDelegate next)
        {
            CreateEssentialCookies(context.HttpContext);
            await next.Invoke();
        }

        private void CreateEssentialCookies(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;

            var settingsCookie = SettingsCookie.CreateFrom(request);
            settingsCookie.WriteTo(response);
        }
    }
}
