using FlightPlanner.Core.Tasks;
using FlightPlanner.Core.Types;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FlightPlanner.Core
{
    public interface IFlightDataSource
    {
        public void Start();
        public bool WaitForData(short timeoutMs, short retryTimeMs);

        public Dictionary<IcaoCode, string>  CurrentMetar { get; }
        public Dictionary<IcaoCode, string>  CurrentTaf { get; }
        public Dictionary<IcaoCode, string>  CurrentNotam { get; }
    }


    public class FlightDataSource :
        IFlightDataSource,
        ITaskSubscriber
    {
        private readonly ILogger<FlightDataSource> _logger;
        private readonly ITaskScheduler _taskScheduler;

        
        public Dictionary<IcaoCode, string>  CurrentMetar { get; private set; }
        public Dictionary<IcaoCode, string>  CurrentTaf { get; private set; }
        public Dictionary<IcaoCode, string>  CurrentNotam { get; private set; }
        

        public FlightDataSource(
            ILogger<FlightDataSource> logger,
            ITaskScheduler taskScheduler
            )
        {
            _logger = logger;
            _taskScheduler = taskScheduler;
            
            CurrentMetar = null;
            CurrentTaf = null;
            CurrentNotam = null;
        }


        public void Start()
        {
            _taskScheduler.SubscribeTo<FetchMetar>(this);
            _taskScheduler.SubscribeTo<FetchTaf>(this);
            _taskScheduler.SubscribeTo<FetchNotam>(this);
        }

        public bool WaitForData(short timeoutMs, short retryTimeMs)
        {
            for (int i = 0; i < (timeoutMs / retryTimeMs); i++)
            {
                if (IsAllDataAvailable())
                {
                    return true;
                }

                Thread.Sleep(retryTimeMs);
            }

            return false;
        }

        public void OnTaskFinished(TaskResult result)
        {
            try
            {
                if (result.TaskType == typeof(FetchMetar))
                    CurrentMetar = result.Data as Dictionary<IcaoCode, string> ;
                if (result.TaskType == typeof(FetchTaf))
                    CurrentTaf = result.Data as Dictionary<IcaoCode, string> ;
                if (result.TaskType == typeof(FetchNotam))
                    CurrentNotam = result.Data as Dictionary<IcaoCode, string> ;
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Failed to process task result");
            }
        }

        
        private bool IsAllDataAvailable()
        {
            return
                CurrentMetar != null &&
                CurrentTaf != null &&
                CurrentNotam != null;
        }
    }
}
