using Microsoft.Extensions.Logging;
using System;

using FlightPlanner.Core.Tasks;
using FlightPlanner.Core.Types;


namespace FlightPlanner.Core
{
    public interface IFlightDataCollector
    {
        public void Start();
        public bool IsDataAvailable();

        public AirportReportMap GetCurrentMetar();
        public AirportReportMap GetCurrentTaf();
        public AirportReportMap GetCurrentNotam();
    }


    public class FlightDataCollector :
        IFlightDataCollector,
        ITaskSubscriber
    {
        private readonly ILogger<FlightDataCollector> _logger;
        private readonly ITaskScheduler _taskScheduler;

        private AirportReportMap _currentMetar;
        private AirportReportMap _currentTaf;
        private AirportReportMap _currentNotam;


        public FlightDataCollector(
            ILogger<FlightDataCollector> logger,
            ITaskScheduler taskScheduler
            )
        {
            _logger = logger;
            _taskScheduler = taskScheduler;

            _currentMetar = null;
            _currentTaf = null;
            _currentNotam = null;
        }


        // IFlightDataCache
        public void Start()
        {
            _taskScheduler.SubscribeTo<FetchMetar>(this);
            _taskScheduler.SubscribeTo<FetchTaf>(this);
            _taskScheduler.SubscribeTo<FetchNotam>(this);
        }

        public bool IsDataAvailable()
        {
            return
                _currentMetar != null && 
                _currentTaf   != null &&
                _currentNotam != null;
        }

        public AirportReportMap GetCurrentMetar() => _currentMetar;
        public AirportReportMap GetCurrentTaf() => _currentTaf;
        public AirportReportMap GetCurrentNotam() => _currentNotam;


        // ITaskSubscriber
        public void OnTaskFinished(TaskResult result)
        {
            try
            {
                if (result.TaskType == typeof(FetchMetar))
                    _currentMetar = result.Data as AirportReportMap;
                if (result.TaskType == typeof(FetchTaf))
                    _currentTaf = result.Data as AirportReportMap;
                if (result.TaskType == typeof(FetchNotam))
                    _currentNotam = result.Data as AirportReportMap;
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Failed to process task result");
            }
        }
    }
}
