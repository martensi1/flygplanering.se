using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightPlanner.Service.Filters
{
    public class RejectFilter : IAsyncPageFilter
    {
        private readonly List<string> _bannedUserAgents;

        
        public RejectFilter()
        {
            _bannedUserAgents = new List<string>(){
                "python",
                "Scrapy",
                "Go-http-client",
                "Java",
                "ruby",
                "github",
                "lua-resty-http"
            };
        }
        
        
        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            await Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(
            PageHandlerExecutingContext context,
            PageHandlerExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            string userAgent = request.Headers["User-Agent"];

            if (IsBanned(userAgent))
            {
                context.Result = new NotFoundResult();
            }
            else
            {
                await next.Invoke();
            }
        }
    
        
        private bool IsBanned(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return true;

            userAgent = userAgent.ToLower();

            foreach (string banned in _bannedUserAgents)
            {
                if (banned.ToLower().StartsWith(userAgent))
                    return true;
            }

            return false;
        }
    }
}
