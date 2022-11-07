using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using FlightPlanner.Core;
using FlightPlanner.Core.Types;
using System.Collections.Generic;

namespace FlightPlanner.Service.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IFlightDataCollector _dataCollector;


        public AirportReportMap CurrentMetar;
        public AirportReportMap CurrentTaf;
        public AirportReportMap CurrentNotam;

        public AirportsCookie MetarCookie;
        public AirportsCookie TafCookie;
        public AirportsCookie NotamCookie;

        public DateTime LastGetUtc { get; private set; }


        public IndexModel(IFlightDataCollector dataCollector)
        {
            _dataCollector = dataCollector;
        }


        public IActionResult OnGet()
        {
            if (!WaitForData(10000, 50)) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            UpdateCookies();
            SetDataRetrievedTime();

            return Page();
        }

        private void UpdateCookies()
        {
            MetarCookie = new AirportsCookie(Request.Cookies["metar-airports"], new List<string>() { "ESGJ", "ESGG" });
            TafCookie = new AirportsCookie(Request.Cookies["taf-airports"], new List<string>() { "ESGJ", "ESGG" });
            NotamCookie = new AirportsCookie(Request.Cookies["notam-airports"], new List<string>() { "ESGJ", "ESGG" });

            Response.Cookies.Append("metar-airports", MetarCookie.ToCookie());
            Response.Cookies.Append("taf-airports", TafCookie.ToCookie());
            Response.Cookies.Append("notam-airports", NotamCookie.ToCookie());
        }

        private bool WaitForData(short timeoutMs, short retryTimeMs)
        {
            for (int i = 0; i < (timeoutMs / retryTimeMs); i++)
            {
                if (_dataCollector.IsDataAvailable())
                {
                    CurrentMetar = _dataCollector.GetCurrentMetar();
                    CurrentTaf = _dataCollector.GetCurrentTaf();
                    CurrentNotam = _dataCollector.GetCurrentNotam();

                    return true;
                }

                Thread.Sleep(retryTimeMs);
            }

            return false;
        }

        private void SetDataRetrievedTime()
        {
            LastGetUtc = DateTime.Now.ToUniversalTime();
        }
    }
}
