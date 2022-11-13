using System;

namespace FlightPlanner.Core.Tasks
{
    public abstract class TaskBase
    {
        public TaskBase(string name)
        {
            if (name.Trim().Length == 0) 
                throw new ArgumentException("Invalid task name, must not be empty");

            Name = name;
        }

        public string Name { get; private set; }


        public TaskResult Execute()
        {
            object data = Run();

            TaskResult result;
            result.TaskType = this.GetType();
            result.Data = data as object;

            return result;
        }

        public override string ToString()
        {
            return Name;
        }


        protected abstract object Run();
    }
}
