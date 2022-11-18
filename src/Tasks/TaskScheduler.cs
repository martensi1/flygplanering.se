using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace FlightPlanner.Service.Tasks
{
    public interface ITaskScheduler
    {
        public void Schedule(TaskBase task, int intervalSeconds);
        public void SubscribeTo<T>(ITaskSubscriber observer);
    }


    public sealed class TaskScheduler :
        ITaskScheduler
    {
        private readonly ILogger<TaskScheduler> _logger;

        private struct ScheduledTask
        {
            public TaskBase Task;
            public Timer Timer;
        }
        private readonly Dictionary<Type, ScheduledTask> _tasks;
        private readonly Dictionary<Type, List<ITaskSubscriber>> _observers;


        public TaskScheduler(
                ILogger<TaskScheduler> logger
            )
        {
            _logger = logger;
            _tasks = new Dictionary<Type, ScheduledTask>();
            _observers = new Dictionary<Type, List<ITaskSubscriber>>();
        }

        ~TaskScheduler()
        {
            _tasks.Select(kvp => kvp.Value).ToList()
                .ForEach(t => t.Timer.Dispose());
        }


        public void Schedule(TaskBase task, int intervalSeconds)
        {
            Type type = task.GetType();

            if (_tasks.ContainsKey(type))
                throw new ArgumentException("Task has already been scheduled");

            _tasks.Add(type, new ScheduledTask {
                Task = task,
                Timer = CreateTimer(task, intervalSeconds)
            });

            _logger.LogInformation("Task scheduled, name: {0}, interval: {1} s", task.Name, intervalSeconds);
        }

        public void SubscribeTo<TTask>(ITaskSubscriber observer)
        {
            Type type = typeof(TTask);

            if (!type.IsSubclassOf(typeof(TaskBase)))
                throw new ArgumentException("Type is not a valid task");

            if (!_observers.ContainsKey(type))
                _observers.Add(type, new List<ITaskSubscriber>());
            
            _observers[type].Add(observer);
        }


        private Timer CreateTimer(TaskBase task, int intervalSeconds)
        {
            TimerCallback callback = CreateTimerCallback(task);
            int intervalMilliseconds = intervalSeconds * 1000;
            
            return new Timer(callback, null, 0, intervalMilliseconds);
        }

        private void NotifyObservers(Type type, TaskResult result)
        {
            if (_observers.ContainsKey(type))
            {
                foreach (var observer in _observers[type])
                {
                    observer.OnTaskFinished(result);
                }
            }
        }

        private TimerCallback CreateTimerCallback(TaskBase task)
        {
            var callback = new TimerCallback(state =>
            {
                try
                {
                    var watch = Stopwatch.StartNew();
                    TaskResult result = task.Execute();

                    watch.Stop();
                    _logger.LogInformation("Task finished, name: {0}, duration: {1} ms", task.Name, watch.ElapsedMilliseconds);

                    NotifyObservers(task.GetType(), result);    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Task execution failed, name: {0}", task.Name);
                }
            });

            return callback;
        }
    }
}
