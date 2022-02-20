using System;
using System.Diagnostics;


namespace FPSE.Core.Download
{
    [DebuggerDisplay("Task={TaskType.FullName}, Data={Data}")]
    public struct TaskResult
    {
        public Type TaskType;
        public object Data;
    }
}
