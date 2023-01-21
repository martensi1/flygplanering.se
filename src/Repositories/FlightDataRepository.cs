using FlightPlanner.Service.Tasks;
using Microsoft.Extensions.Logging;
using PilotAppLib.Clients.NotamSearch;
using PilotAppLib.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FlightPlanner.Service.Repositories
{
    public interface IFlightDataRepository
    {
        public void Initialize();
        public bool WaitForData(short timeoutMs, short retryTimeMs);

        public Dictionary<IcaoCode, string>  CurrentMetar { get; }
        public Dictionary<IcaoCode, string>  CurrentTaf { get; }
        public Dictionary<IcaoCode, List<NotamRecord>>  CurrentNotam { get; }
    }


    public class FlightDataRepository :
        IFlightDataRepository
    {
        private readonly ILogger<FlightDataRepository> _logger;
        private readonly ITaskScheduler _taskScheduler;


        public Dictionary<IcaoCode, string>  CurrentMetar { get; private set; }
        public Dictionary<IcaoCode, string>  CurrentTaf { get; private set; }
        public Dictionary<IcaoCode, List<NotamRecord>>  CurrentNotam { get; private set; }


        public FlightDataRepository(
            ILogger<FlightDataRepository> logger,
            ITaskScheduler taskScheduler
            )
        {
            _logger = logger;
            _taskScheduler = taskScheduler;

            CurrentMetar = null;
            CurrentTaf = null;
            CurrentNotam = null;
        }


        public void Initialize()
        {
            _taskScheduler.OnTaskSuccess += OnTaskFinished;
            _taskScheduler.OnTaskFailure += OnTaskFailed;
        }

        public void OnTaskFinished(object sender, TaskExecutedEventArgs e)
        {
            try
            {
                if (e.TaskType == typeof(FetchMetar))
                    CurrentMetar = e.Result as Dictionary<IcaoCode, string>;
                if (e.TaskType == typeof(FetchTaf))
                    CurrentTaf = e.Result as Dictionary<IcaoCode, string>;
                if (e.TaskType == typeof(FetchNotam))
                    CurrentNotam = e.Result as Dictionary<IcaoCode, List<NotamRecord>>;
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Failed to process task result");
            }
        }

        public void OnTaskFailed(object sender, TaskExecutedEventArgs e)
        {
            if (e.TaskType == typeof(FetchNotam))
            {
                // Because the corectness of NOTAM is highly safety critical, clear the
                // old NOTAM data on failed fetch
                CurrentNotam.Clear();
            }
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


        private bool IsAllDataAvailable()
        {
            return
                CurrentMetar != null &&
                CurrentTaf != null &&
                CurrentNotam != null;
        }
    }
}
