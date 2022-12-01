using System;

namespace FlightPlanner.Service.Tasks
{
    public class TaskExecutedEventArgs : EventArgs
    {
        public Type TaskType { get; set; }
        public string TaskName { get; set; }

        public object Result { get; set; }
        public long DurationMilliseconds { get; set; }

        public Exception ThrownException { get; set; }
    }

    public delegate void TaskExecutedEventHandler(
            object sender, TaskExecutedEventArgs e);
}
