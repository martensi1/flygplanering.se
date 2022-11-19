using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightPlanner.Service.Filters
{
    public class OrganizationFilter : IAsyncPageFilter
    {
        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            await Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(
            PageHandlerExecutingContext context,
            PageHandlerExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            string displayUrl = request.GetDisplayUrl();

            if (HasSubdomain(displayUrl, "jfk"))
            {
                context.HttpContext.Items.Add("JFK", "");
            }

            await next.Invoke();
        }
    
        
        private bool HasSubdomain(string url, string subdomain)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            string pattern = "^(http:\\/\\/|https:\\/\\/|)(www\\.|)" + subdomain + "\\.";
            return Regex.IsMatch(url, pattern);
        }
    }
}
