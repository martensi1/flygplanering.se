using System;

namespace FlightPlanner.Service.Tasks
{
    public abstract class TaskBase
    {
        public string Name { get; private set; }

        public abstract object Run();


        public TaskBase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Task name cannot be empty");
            }

            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
