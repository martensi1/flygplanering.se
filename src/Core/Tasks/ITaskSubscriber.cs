namespace FlightPlanner.Core.Tasks
{
    public interface ITaskSubscriber
    {
        void OnTaskFinished(TaskResult result);
    }
}
