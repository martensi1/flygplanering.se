using System;

namespace FlightPlanner.Service.Tasks
{
    public interface ITaskSubscriber
    {
        void OnTaskFinished(TaskResult result);
    }
}
