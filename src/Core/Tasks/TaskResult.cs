using System;
using System.Diagnostics;

namespace FlightPlanner.Core.Tasks
{
    [DebuggerDisplay("Task={TaskType.FullName}, Data={Data}")]
    public struct TaskResult
    {
        public Type TaskType;
        public object Data;
    }
}
