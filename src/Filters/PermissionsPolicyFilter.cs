using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightPlanner.Service.Filters
{
    public class PermissionsPolicyFilter : IAsyncAlwaysRunResultFilter
    {
        private readonly List<string> _disabledFeatures;


        public PermissionsPolicyFilter()
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
            string policyString = BuildPolicyString();

            response.Headers["Feature-Policy"] = policyString;
            response.Headers["Permissions-Policy"] = policyString;

            await next.Invoke();
        }

        private string BuildPolicyString()
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
