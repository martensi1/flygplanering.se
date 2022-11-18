using FlightPlanner.Service.Models;
using FlightPlanner.Service.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PilotAppLib.Clients.NotamSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FlightPlanner.Service.Pages
{
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.None)]
    public class IndexModel : PageModel
    {
        private readonly IFlightDataRepository _dataRepository;


        public IndexModel(IFlightDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }


        public string CurrentTimeUtc
        {
            get {
                return DateTime.Now.ToUniversalTime()
                .ToString("HH:mm (UTC), dd-MM-yyyy");
            }
        }

        public bool ShowWeightBalanceSection
        {
            get
            {
                string displayUrl = HttpContext.Request.GetDisplayUrl();
                string expectedSubdomain = "jfk";

                return Regex.IsMatch(displayUrl,
                    "^(http:\\/\\/|https:\\/\\/|)(www\\.|)" + expectedSubdomain + "\\.");
            }
        }

        public bool SettingsSaved { get; private set; }

        public Dictionary<IcaoCode, string> Metar { get; private set; }
        public Dictionary<IcaoCode, string> Taf { get; private set; }
        public Dictionary<IcaoCode, List<NotamRecord>> Notam { get; private set; }



        public IActionResult OnGet()
        {
            SettingsSaved = TempData.Remove("SettingsSaved");

            if (!GetAndFilterData()) 
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            return Page();
        }

        private bool GetAndFilterData()
        {
            if (!_dataRepository.WaitForData(10000, 50))
            {
                return false;
            }

            var settingsCookie = SettingsCookie.CreateFrom(Request);
            
            Metar = SortOutUnwantedAirports(_dataRepository.CurrentMetar, settingsCookie.MetarAirports);
            Taf = SortOutUnwantedAirports(_dataRepository.CurrentTaf, settingsCookie.TafAirports);
            Notam = SortOutUnwantedAirports(_dataRepository.CurrentNotam, settingsCookie.NotamAirports);

            settingsCookie.WriteTo(Response);
            return true;
        }
        
        private Dictionary<IcaoCode, TValue> SortOutUnwantedAirports<TValue>(
            Dictionary<IcaoCode, TValue> reportMap, IReadOnlyList<IcaoCode> wantedAirports)
        {
            var filteredDictionary = reportMap.Where(r => wantedAirports.Contains(r.Key))
                .ToDictionary(dict => dict.Key, dict => dict.Value);

            var sortedDictionary = (from entry in filteredDictionary orderby entry.Key ascending select entry)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            return sortedDictionary;
        }
    }
}
