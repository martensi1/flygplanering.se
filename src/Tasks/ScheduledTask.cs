using System;
using System.Diagnostics;
using System.Threading;

namespace FlightPlanner.Service.Tasks
{
    public class ScheduledTask : IDisposable
    {
        private readonly Timer _timer;

        public bool Status { get; private set; }

        public event TaskExecutedEventHandler OnExecuted;


        public ScheduledTask(TaskBase task, int intervalMilliseconds)
        {
            Status = true;
            _timer = CreateTimer(task, intervalMilliseconds);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }


        private Timer CreateTimer(TaskBase task, int intervalMilliseconds)
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
                    Status = true;
                }
                catch (Exception ex)
                {
                    eventArguments.ThrownException = ex;
                    Status = false;
                }

                watch.Stop();
                eventArguments.DurationMilliseconds = watch.ElapsedMilliseconds;

                OnExecuted?.Invoke(this, eventArguments);
            });

            return new Timer(callback, null, 0, intervalMilliseconds);
        }
    }
}
