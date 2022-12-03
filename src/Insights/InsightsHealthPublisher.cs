using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Insights
{
    public class InsightsHealthPublisher : IHealthCheckPublisher
    {
        private readonly TelemetryClient _client;


        public InsightsHealthPublisher(ApplicationInsightsServiceOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            string connectionString = options.ConnectionString;
            _client = CreateClient(connectionString);
        }

        public Task PublishAsync(
            HealthReport report,
            CancellationToken cancellationToken = default
            )
        {
            if (report.Status == HealthStatus.Healthy)
            {
                return Task.CompletedTask;
            }

            PublishCustomEvent(report);
            return Task.CompletedTask;
        }

        public void PublishCustomEvent(HealthReport report)
        {
            foreach (var entry in report.Entries)
            {
                string healthCheckName = entry.Key;
                string healthCheckStatus = entry.Value.Status.ToString();
                double healthCheckStatusCode = entry.Value.Status == HealthStatus.Healthy ? 0 : 1;
                double healthCheckDuration = entry.Value.Duration.TotalMilliseconds;

                string machineName = Environment.MachineName;
                string assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;

                _client.TrackEvent($"HealthCheckAlert:{healthCheckName}",
                    properties: new Dictionary<string, string>()
                    {
                        { "Health status", healthCheckStatus },
                        { "Machine name", machineName },
                        { "Assembly", assemblyName },
                    },
                    metrics: new Dictionary<string, double>()
                    {
                        { "Health status code", healthCheckStatusCode },
                        { "Health check duration (ms)", healthCheckDuration }
                    });
            }
        }

        private TelemetryClient CreateClient(string connectionString)
        {
            var configuration = new TelemetryConfiguration {
                ConnectionString = connectionString
            };

            return new TelemetryClient(configuration);
        }
    }
}
