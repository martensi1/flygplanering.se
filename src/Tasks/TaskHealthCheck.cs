using FlightPlanner.Service.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Tasks
{
    public class TaskHealthCheck : IHealthCheck
    {
        private readonly ILogger<TaskHealthCheck> _logger;
        private readonly ITaskScheduler _taskScheduler;

        public TaskHealthCheck(
            ILogger<TaskHealthCheck> logger,
            ITaskScheduler taskScheduler
            )
        {
            _logger = logger;
            _taskScheduler = taskScheduler;
        }


        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
            )
        {
            bool isHealthy = _taskScheduler.AreAllTasksHealthy();
            _logger.LogInformation("Task health check, healthy: {0}", isHealthy);

            return isHealthy ?
                Task.FromResult(HealthCheckResult.Healthy("All tasks are healthy")) :
                Task.FromResult(HealthCheckResult.Unhealthy("Some tasks are unhealthy"));
        }
    }
}
