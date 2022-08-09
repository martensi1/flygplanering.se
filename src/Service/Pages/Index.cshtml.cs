using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Threading;

using FlightPlanner.Core;
using FlightPlanner.Core.Types;


namespace FlightPlanner.Service.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IFlightDataCollector _dataCollector;


        public AirportReportMap CurrentMetar;
        public AirportReportMap CurrentTaf;
        public AirportReportMap CurrentNotam;

        public DateTime LastGetUtc { get; private set; }


        public IndexModel(IFlightDataCollector dataCollector)
        {
            _dataCollector = dataCollector;
        }


        public void OnGet()
        {
            LastGetUtc = DateTime.Now.ToUniversalTime();
            WaitForData(10000, 50);
        }

        private void WaitForData(short timeoutMs, short retryTimeMs)
        {
            for (int i = 0; i < (timeoutMs / retryTimeMs); i++)
            {
                if (_dataCollector.IsDataAvailable())
                {
                    CurrentMetar = _dataCollector.GetCurrentMetar();
                    CurrentTaf = _dataCollector.GetCurrentTaf();
                    CurrentNotam = _dataCollector.GetCurrentNotam();

                    break;
                }

                Thread.Sleep(retryTimeMs);
            }
        }
    }
}
