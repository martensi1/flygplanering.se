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

            AddStrictTransportSecurityHeader(response);
            AddPermissionsPolicyHeader(response);
            AddXXSSProtectionHeader(response);
            AddContentTypeOptions(response);
            AddFrameOptions(response);

            await next.Invoke();
        }


        private void AddStrictTransportSecurityHeader(HttpResponse response)
        {
            string headerValue = "max-age=63072000; includeSubDomains; preload";
            response.Headers[HeaderNames.StrictTransportSecurity] = headerValue;
        }

        private void AddXXSSProtectionHeader(HttpResponse response)
        {
            string headerValue = "X-XSS-Protection: 1; mode=block";
            response.Headers[HeaderNames.XXSSProtection] = headerValue;
        }

        private void AddContentTypeOptions(HttpResponse response)
        {
            string headerValue = "nosniff";
            response.Headers[HeaderNames.XContentTypeOptions] = headerValue;
        }

        private void AddFrameOptions(HttpResponse response)
        {
            string headerValue = "DENY";
            response.Headers[HeaderNames.XFrameOptions] = headerValue;
        }

        private void AddPermissionsPolicyHeader(HttpResponse response)
        {
            string headerValue = BuildPermissionsPolicyString();

            response.Headers["Feature-Policy"] = headerValue;
            response.Headers["Permissions-Policy"] = headerValue;
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
