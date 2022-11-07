using FlightPlanner.Core;
using FlightPlanner.Core.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightPlanner.Service.Pages
{
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.None)]
    public class IndexModel : PageModel
    {
        private readonly IFlightDataSource _dataSource;


        public IndexModel(IFlightDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        
        public Dictionary<IcaoCode, string> Metar { get; private set; }
        public Dictionary<IcaoCode, string> Taf { get; private set; }
        public Dictionary<IcaoCode, string> Notam { get; private set; }

        public DateTime LastGetUtc { get; private set; }


        public IActionResult OnGet()
        {
            if (!_dataSource.WaitForData(10000, 50)) 
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            UpdateCookies();
            SetDataRetrievedTime();

            return Page();
        }

        private void UpdateCookies()
        {
            var metarAirports = CookieParsing.ToAirportList(Request.Cookies["metar-airports"], new List<IcaoCode>() { "ESGJ", "ESGG" });
            var tafAirports = CookieParsing.ToAirportList(Request.Cookies["taf-airports"], new List<IcaoCode>() { "ESGJ", "ESGG" });
            var notamAirports = CookieParsing.ToAirportList(Request.Cookies["notam-airports"], new List<IcaoCode>() { "ESGJ" });

            Metar = SortOutUnwantedAirports(_dataSource.CurrentMetar, metarAirports);
            Taf = SortOutUnwantedAirports(_dataSource.CurrentTaf, tafAirports);
            Notam = SortOutUnwantedAirports(_dataSource.CurrentNotam, notamAirports);

            Response.Cookies.Append("metar-airports", CookieParsing.FromAirportList(metarAirports));
            Response.Cookies.Append("taf-airports", CookieParsing.FromAirportList(tafAirports));
            Response.Cookies.Append("notam-airports", CookieParsing.FromAirportList(notamAirports));
        }

        private Dictionary<IcaoCode, string> SortOutUnwantedAirports(
            Dictionary<IcaoCode, string> reportMap, IReadOnlyList<IcaoCode> wantedAirports)
        {
            return reportMap.Where(r => wantedAirports.Contains(r.Key))
                .ToDictionary(dict => dict.Key, dict => dict.Value);
        }

        private void SetDataRetrievedTime()
        {
            LastGetUtc = DateTime.Now.ToUniversalTime();
        }
    }
}
