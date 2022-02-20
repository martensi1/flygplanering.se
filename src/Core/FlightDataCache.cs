using FPSE.Core.Download;
using FPSE.Core.Download.Tasks;
using FPSE.Core.Types;
using Microsoft.Extensions.Logging;
using System;


namespace FPSE.Core
{
    public interface IFlightDataCache
    {
        public void Initialize();
        public bool IsDataAvailable();

        public AirportReports GetCurrentMetar();
        public AirportReports GetCurrentTaf();
        public AirportReports GetCurrentNotam();
    }


    public class FlightDataCache : 
        IFlightDataCache,
        IDownloadObserver
    {
        private readonly ILogger<FlightDataCache> _logger;
        private readonly IDownloadScheduler _downloadScheduler;

        private AirportReports _currentMetar;
        private AirportReports _currentTaf;
        private AirportReports _currentNotam;


        public FlightDataCache(
            ILogger<FlightDataCache> logger,
            IDownloadScheduler downloadScheduler
            )
        {
            _logger = logger;
            _downloadScheduler = downloadScheduler;

            _currentMetar = null;
            _currentTaf = null;
            _currentNotam = null;
        }


        // IFlightDataCache
        public void Initialize()
        {
            _downloadScheduler.Observe<MetarDownload>(this);
            _downloadScheduler.Observe<TafDownload>(this);
            _downloadScheduler.Observe<NotamDownload>(this);
        }

        public bool IsDataAvailable()
        {
            return
                _currentMetar != null && 
                _currentTaf   != null &&
                _currentNotam != null;
        }

        public AirportReports GetCurrentMetar() => _currentMetar;
        public AirportReports GetCurrentTaf() => _currentTaf;
        public AirportReports GetCurrentNotam() => _currentNotam;


        // IDownloadObserver
        public void OnDownloadFinished(TaskResult result)
        {
            try
            {
                if (result.TaskType == typeof(MetarDownload))
                    _currentMetar = result.Data as AirportReports;
                if (result.TaskType == typeof(TafDownload))
                    _currentTaf = result.Data as AirportReports;
                if (result.TaskType == typeof(NotamDownload))
                    _currentNotam = result.Data as AirportReports;
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Failed to process download task result");
            }
        }
    }
}
