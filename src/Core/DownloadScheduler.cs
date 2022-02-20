using FPSE.Core.Download;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace FPSE.Core
{
    public interface IDownloadScheduler
    {
        public void Schedule(DownloadTask task, int intervalMilliseconds);
        public void Observe<T>(IDownloadObserver observer);
    }


    public sealed class DownloadScheduler :
        IDownloadScheduler
    {
        private readonly ILogger<DownloadScheduler> _logger;

        private struct ScheduledTask
        {
            public DownloadTask Task;
            public Timer Timer;
            public List<IDownloadObserver> Observers;
        }
        private readonly Dictionary<Type, ScheduledTask> _tasks;


        public DownloadScheduler(
                ILogger<DownloadScheduler> logger
            )
        {
            _logger = logger;
            _tasks = new Dictionary<Type, ScheduledTask>();
        }

        ~DownloadScheduler()
        {
            _tasks.Select(kvp => kvp.Value).ToList()
                .ForEach(t => t.Timer.Dispose());
        }


        public void Schedule(DownloadTask task, int intervalMilliseconds)
        {
            Type type = task.GetType();

            if (_tasks.ContainsKey(type))
                throw new ArgumentException("Task has already been scheduled");

            _tasks.Add(type, new ScheduledTask {
                Task = task,
                Timer = CreateTimer(task, intervalMilliseconds),
                Observers = new List<IDownloadObserver>()
            });
        }

        public void Observe<TTask>(IDownloadObserver observer)
        {
            Type type = typeof(TTask);

            if (!_tasks.ContainsKey(type))
                throw new ArgumentException("No registered task of type found");

            _tasks[type].Observers.Add(observer);
        }


        private Timer CreateTimer(DownloadTask task, int intervalMilliseconds)
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
                    observer.OnDownloadFinished(result);
                }
            }
        }
    }
}
