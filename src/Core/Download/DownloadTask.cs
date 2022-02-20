using System;


namespace FPSE.Core.Download
{
    public abstract class DownloadTask
    {
        public DownloadTask(string name)
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
