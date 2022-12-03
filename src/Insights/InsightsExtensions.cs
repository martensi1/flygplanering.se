using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace Service.Insights
{
    public static class InsightsExtensions
    {
        public static IServiceCollection AddApplicationInsights(
            this IServiceCollection services
            )
        {
            var options = CreateOptions();

            services.AddApplicationInsightsTelemetry(options);
            services.AddSingleton<IHealthCheckPublisher>(new InsightsHealthPublisher(options));

            return services;
        }

        private static ApplicationInsightsServiceOptions CreateOptions()
        {
            string connectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

            var options = new ApplicationInsightsServiceOptions() {
                ConnectionString = connectionString
            };

            return options;
        }
    }
}
