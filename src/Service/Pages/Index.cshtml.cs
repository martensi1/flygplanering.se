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

        public string RetrievedTimeUtc { get; private set; }
        public string NswcUrl { get; private set; }


        public IActionResult OnGet()
        {
            if (!_dataSource.WaitForData(10000, 50)) 
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            GetAndFilterData();
            SetDataRetrievedTime();
            CreateNonCacheableNswcUrl();

            return Page();
        }

        private void GetAndFilterData()
        {
            var settingsCookie = SettingsCookie.CreateFrom(Request);

            Metar = SortOutUnwantedAirports(_dataSource.CurrentMetar, settingsCookie.MetarAirports);
            Taf = SortOutUnwantedAirports(_dataSource.CurrentTaf, settingsCookie.TafAirports);
            Notam = SortOutUnwantedAirports(_dataSource.CurrentNotam, settingsCookie.NotamAirports);

            settingsCookie.WriteTo(Response);
        }

        private Dictionary<IcaoCode, string> SortOutUnwantedAirports(
            Dictionary<IcaoCode, string> reportMap, IReadOnlyList<IcaoCode> wantedAirports)
        {
            return reportMap.Where(r => wantedAirports.Contains(r.Key))
                .ToDictionary(dict => dict.Key, dict => dict.Value);
        }

        private void SetDataRetrievedTime()
        {
            RetrievedTimeUtc = DateTime.Now.ToUniversalTime()
                .ToString("HH:mm (UTC), dd-MM-yyyy");
        }

        private void CreateNonCacheableNswcUrl()
        {
            var guid = Guid.NewGuid();
            NswcUrl = "https://aro.lfv.se/tor/nswc2aro.gif?" + guid.ToString();
        }
    }
}
