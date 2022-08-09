using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using FlightPlanner.Core.Tasks;


namespace FlightPlanner.Core
{
    public interface ITaskScheduler
    {
        public void Schedule(TaskBase task, int intervalMilliseconds);
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
            public List<ITaskSubscriber> Observers;
        }
        private readonly Dictionary<Type, ScheduledTask> _tasks;


        public TaskScheduler(
                ILogger<TaskScheduler> logger
            )
        {
            _logger = logger;
            _tasks = new Dictionary<Type, ScheduledTask>();
        }

        ~TaskScheduler()
        {
            _tasks.Select(kvp => kvp.Value).ToList()
                .ForEach(t => t.Timer.Dispose());
        }


        public void Schedule(TaskBase task, int intervalMilliseconds)
        {
            Type type = task.GetType();

            if (_tasks.ContainsKey(type))
                throw new ArgumentException("Task has already been scheduled");

            _tasks.Add(type, new ScheduledTask {
                Task = task,
                Timer = CreateTimer(task, intervalMilliseconds),
                Observers = new List<ITaskSubscriber>()
            });
        }

        public void SubscribeTo<TTask>(ITaskSubscriber observer)
        {
            Type type = typeof(TTask);

            if (!_tasks.ContainsKey(type))
                throw new ArgumentException("No registered task of type found");

            _tasks[type].Observers.Add(observer);
        }


        private Timer CreateTimer(TaskBase task, int intervalMilliseconds)
        {
            var callback = new TimerCallback(state =>
            {
                TaskResult result = task.Execute();
                NotifyObservers(task.GetType(), result);
            });

            return new Timer(callback, null, 0, intervalMilliseconds);
        }

        private void NotifyObservers(Type type, TaskResult result)
        {
            if (_tasks.ContainsKey(type))
            {
                foreach (var observer in _tasks[type].Observers)
                {
                    observer.OnTaskFinished(result);
                }
            }
        }
    }
}
