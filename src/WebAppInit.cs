using FlightPlanner.Service.Common;
using FlightPlanner.Service.Config;
using FlightPlanner.Service.Repositories;
using FlightPlanner.Service.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using PilotAppLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightPlanner.Service
{
    public class WebAppInit : IStartupFilter
    {
        private readonly ILogger<WebAppInit> _logger;
        private readonly ITaskScheduler _taskScheduler;
        private readonly IFlightDataRepository _dataRepository;


        public WebAppInit(
            ILogger<WebAppInit> logger,
            ITaskScheduler taskScheduler,
            IFlightDataRepository dataRepository
            )
        {
            _logger = logger;
            _taskScheduler = taskScheduler;
            _dataRepository = dataRepository;
    }


        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            StartDataFetch();
            return next;
        }

        private void StartDataFetch()
        {
            _dataRepository.Initialize();
            var supportedAirports = LoadAirportConfig();


            _taskScheduler.Schedule(new FetchMetar(supportedAirports),
                Constants.MetarFetchIntervalSeconds);

            _taskScheduler.Schedule(new FetchTaf(supportedAirports),
                Constants.TafFetchIntervalSeconds);

            _taskScheduler.Schedule(new FetchNotam(supportedAirports),
                Constants.NotamFetchIntervalSeconds);
        }

        private List<IcaoCode> LoadAirportConfig()
        {
            var supportedAirports = AirportConfig.ReadToList();

            _logger.LogInformation("Airport config loaded, airports: {0}",
                string.Join(", ", supportedAirports.Select(i => i.ToString()).ToArray()));

            return supportedAirports;
        }
    }
}
