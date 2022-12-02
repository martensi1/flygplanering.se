using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FlightPlanner.Service.Tasks
{
    public interface ITaskScheduler
    {
        public void Schedule(TaskBase task, int intervalSeconds);

        public bool AreAllTasksHealthy();

        public event TaskExecutedEventHandler OnTaskExecuted;
    }


    public sealed class TaskScheduler :
        ITaskScheduler,
        IDisposable
    {
        private readonly ILogger<TaskScheduler> _logger;
        private readonly List<ScheduledTask> _taskList;

        public event TaskExecutedEventHandler OnTaskExecuted;


        public TaskScheduler(
                ILogger<TaskScheduler> logger
            )
        {
            _logger = logger;
            _taskList = new List<ScheduledTask>();
        }

        public void Dispose()
        {
            _taskList.ForEach(t => t.Dispose());
            _taskList.Clear();
        }


        public void Schedule(TaskBase task, int intervalSeconds)
        {
            var scheduledTask = new ScheduledTask(task, intervalSeconds);
            scheduledTask.OnExecuted += OnScheduledTaskExecuted;

            _taskList.Add(scheduledTask);
            _logger.LogInformation("Task scheduled, name: {0}, interval: {1} s", task.Name, intervalSeconds);
        }

        public bool AreAllTasksHealthy()
        {
            return _taskList.TrueForAll(t => t.Status);
        }


        private void OnScheduledTaskExecuted(object sender, TaskExecutedEventArgs e)
        {
            if (e.ThrownException != null)
            {
                _logger.LogError(e.ThrownException, "Task execution failed, name: {0}", e.TaskName);
                return;
            }

            _logger.LogInformation("Task finished, name: {0}, duration: {1} ms", e.TaskName, e.DurationMilliseconds);
            OnTaskExecuted?.Invoke(this, e);
        }
    }
}
