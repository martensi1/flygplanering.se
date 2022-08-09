using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using FlightPlanner.Core;
using FlightPlanner.Core.Tasks;
using FlightPlanner.Core.Types;


namespace FlightPlanner.Service
{
    public class WebAppInit : IStartupFilter
    {
        private readonly ILogger<WebAppInit> _logger;

        private readonly ITaskScheduler _taskScheduler;
        private readonly IFlightDataCollector _dataCollector;

        private IReadOnlyList<IcaoCode> _weatherAirports;
        private IReadOnlyList<IcaoCode> _notamAirports;


        public WebAppInit(
            ILogger<WebAppInit> logger,
            ITaskScheduler taskScheduler,
            IFlightDataCollector dataCollector
            )
        {
            _logger = logger;
            _taskScheduler = taskScheduler;
            _dataCollector = dataCollector;

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
            StartDataCollection();

            return next;
        }


        private void StartDataCollection()
        {
            _dataCollector.Start();
        }

        private void ScheduleDownloads()
        {
            _taskScheduler.Schedule(
                new FetchMetar(_weatherAirports),
                60000
             );

            _taskScheduler.Schedule(
                new FetchTaf(_weatherAirports),
                60000
             );

            _taskScheduler.Schedule(
                new FetchNotam(_notamAirports),
                60000
             );
        }
    }
}
