using FlightPlanner.Core;
using FlightPlanner.Core.Tasks;
using FlightPlanner.Core.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace FlightPlanner.Service
{
    public class WebAppInit : IStartupFilter
    {
        private readonly ILogger<WebAppInit> _logger;

        private readonly ITaskScheduler _taskScheduler;
        private readonly IFlightDataSource _dataSource;


        public WebAppInit(
            ILogger<WebAppInit> logger,
            ITaskScheduler taskScheduler,
            IFlightDataSource dataSource
            )
        {
            _logger = logger;
            _taskScheduler = taskScheduler;
            _dataSource = dataSource;
        }


        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {            
            ScheduleTasks();        
            return next;
        }

        private void ScheduleTasks()
        {
            // load airports.json file and put into a list
            var supportedAirports = AirportConfig.ReadToList();

            _taskScheduler.Schedule(new FetchMetar(supportedAirports), 60);
            _taskScheduler.Schedule(new FetchTaf(supportedAirports), 60);
            _taskScheduler.Schedule(new FetchNotam(supportedAirports), 300);
        }
    }
}
