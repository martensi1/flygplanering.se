using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Middlewares
{
    public class BlockScriptsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly List<string> _bannedUserAgents;


        public BlockScriptsMiddleware(RequestDelegate next)
        {
            _next = next;
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


        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            string userAgent = request.Headers["User-Agent"];

            if (IsBanned(userAgent))
            {
                context.Response.StatusCode = 404;
                return;
            }
            else
            {
                await _next.Invoke(context);
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

    public static class NoCacheMiddlewareExtensions
    {
        public static IApplicationBuilder UseScriptBlocking(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BlockScriptsMiddleware>();
        }
    }
}
