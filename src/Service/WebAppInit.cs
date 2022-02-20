using FPSE.Core;
using FPSE.Core.Download.Tasks;
using FPSE.Core.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;


namespace FPSE.Service
{
    public class WebAppInit : IStartupFilter
    {
        private readonly ILogger<WebAppInit> _logger;

        private readonly IDownloadScheduler _downloadScheduler;
        private readonly IFlightDataCache _flightDataCache;

        private IReadOnlyList<IcaoCode> _weatherAirports;
        private IReadOnlyList<IcaoCode> _notamAirports;


        public WebAppInit(
            ILogger<WebAppInit> logger,
            IDownloadScheduler downloadScheduler,
            IFlightDataCache flightDataCache
            )
        {
            _logger = logger;
            _downloadScheduler = downloadScheduler;
            _flightDataCache = flightDataCache;

            _weatherAirports = new List<IcaoCode> {
                "ESGJ",
                "ESGG",
                "ESMX",
                "ESMT",
                "ESGR",
                "ESSL",
                "ESMS"
            };

            _notamAirports = new List<IcaoCode> {
                "ESGJ"
            };
        }


        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            ScheduleDownloads();
            InitializeCache();

            return next;
        }


        private void InitializeCache()
        {
            _flightDataCache.Initialize();
        }

        private void ScheduleDownloads()
        {
            _downloadScheduler.Schedule(
                new MetarDownload("https://aro.lfv.se/Links/Link/ViewLink?TorLinkId=314&type=MET", _weatherAirports),
                60000
             );

            _downloadScheduler.Schedule(
                new TafDownload("https://aro.lfv.se/Links/Link/ViewLink?TorLinkId=315&type=MET", _weatherAirports),
                60000
             );

            _downloadScheduler.Schedule(
                new NotamDownload("https://notams.aim.faa.gov/notamSearch/search", _notamAirports),
                60000
             );
        }
    }
}
