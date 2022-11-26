using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightPlanner.Service.Filters
{
    public class SecurityHeadersFilter : IAsyncAlwaysRunResultFilter
    {
        private readonly List<string> _disabledFeatures;


        public SecurityHeadersFilter()
        {
            _disabledFeatures = new List<string>() {
                "microphone",
                "speaker",
                "camera",
                "geolocation",
                "display-capture",
                "payment"
            };
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var response = context.HttpContext.Response;
            AddSecurityHeaders(response);

            await next.Invoke();
        }


        private void AddSecurityHeaders(HttpResponse response)
        {
            response.Headers[HeaderNames.StrictTransportSecurity] = "max-age=63072000; includeSubDomains; preload";
            response.Headers[HeaderNames.XContentTypeOptions] = "nosniff";
            response.Headers[HeaderNames.XFrameOptions] = "DENY";
            response.Headers[HeaderNames.XXSSProtection] = "1; mode=block";

            string permissionPolicy = BuildPermissionsPolicyString();
            response.Headers["Feature-Policy"] = permissionPolicy;
            response.Headers["Permissions-Policy"] = permissionPolicy;
        }

        private string BuildPermissionsPolicyString()
        {
            string result = string.Empty;

            foreach (string disabledFeature in _disabledFeatures)
            {
                if (result.Length > 0)
                    result += "; ";

                result += $"{disabledFeature} 'none'";
            }

            return result;
        }
    }
}
