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
        public void StartSubscriptions();
        public bool WaitForData(short timeoutMs, short retryTimeMs);

        public Dictionary<IcaoCode, string>  CurrentMetar { get; }
        public Dictionary<IcaoCode, string>  CurrentTaf { get; }
        public Dictionary<IcaoCode, List<NotamRecord>>  CurrentNotam { get; }
    }


    public class FlightDataRepository :
        IFlightDataRepository,
        ITaskSubscriber
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


        public void StartSubscriptions()
        {
            _taskScheduler.SubscribeTo<FetchMetar>(this);
            _taskScheduler.SubscribeTo<FetchTaf>(this);
            _taskScheduler.SubscribeTo<FetchNotam>(this);

            _logger.LogInformation("Data repository subscribed to fetch tasks");
        }

        public void OnTaskFinished(TaskResult result)
        {
            try
            {
                if (result.TaskType == typeof(FetchMetar))
                    CurrentMetar = result.Data as Dictionary<IcaoCode, string>;
                if (result.TaskType == typeof(FetchTaf))
                    CurrentTaf = result.Data as Dictionary<IcaoCode, string>;
                if (result.TaskType == typeof(FetchNotam))
                    CurrentNotam = result.Data as Dictionary<IcaoCode, List<NotamRecord>>;
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Failed to process task result");
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
