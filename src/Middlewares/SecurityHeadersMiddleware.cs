using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace FlightPlanner.Service.Middlewares
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IReadOnlyList<string> _disabledFeatures;


        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
            _disabledFeatures = new List<string>() {
                "camera",
                "display-capture",
                "geolocation",
                "microphone",
                "payment",
                "usb"
            };
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var response = context.Response;
            AddSecurityHeaders(response);

            await _next.Invoke(context);
        }


        private void AddSecurityHeaders(HttpResponse response)
        {
            response.Headers[HeaderNames.StrictTransportSecurity] = "max-age=63072000; includeSubDomains; preload";
            response.Headers[HeaderNames.XContentTypeOptions] = "nosniff";
            response.Headers[HeaderNames.XFrameOptions] = "DENY";
            response.Headers[HeaderNames.XXSSProtection] = "1; mode=block";

            response.Headers["Feature-Policy"] = BuildFeaturePolicyString();
            response.Headers["Permissions-Policy"] = BuildPermissionsPolicyString(); ;
        }

        private string BuildFeaturePolicyString()
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

        private string BuildPermissionsPolicyString()
        {
            string result = string.Empty;

            foreach (string disabledFeature in _disabledFeatures)
            {
                if (result.Length > 0)
                    result += ", ";

                result += $"{disabledFeature}=()";
            }

            return result;
        }
    }

    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
