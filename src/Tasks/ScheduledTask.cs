using System;
using System.Diagnostics;
using System.Threading;

namespace FlightPlanner.Service.Tasks
{
    public class ScheduledTask : IDisposable
    {
        private readonly Timer _timer;

        public event TaskExecutedEventHandler OnExecuted;


        public ScheduledTask(TaskBase task, int intervalSeconds)
        {
            var callback = CreateTimerCallback(task);
            int intervalMilliseconds = intervalSeconds * 1000;

            _timer = new Timer(callback, null, 0, intervalMilliseconds);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }


        private TimerCallback CreateTimerCallback(TaskBase task)
        {
            var callback = new TimerCallback(state =>
            {
                var eventArguments = new TaskExecutedEventArgs() {
                    TaskName = task.Name,
                    TaskType = task.GetType()
                };

                var watch = Stopwatch.StartNew();

                try
                {
                    eventArguments.Result = task.Run();
                }
                catch (Exception ex)
                {
                    eventArguments.ThrownException = ex;
                }

                watch.Stop();
                eventArguments.DurationMilliseconds = watch.ElapsedMilliseconds;

                OnExecuted?.Invoke(this, eventArguments);
            });

            return callback;
        }
    }
}
