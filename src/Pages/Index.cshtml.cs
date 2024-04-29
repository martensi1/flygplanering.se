using FlightPlanner.Service.Extensions;
using FlightPlanner.Service.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PilotAppLib.Clients.NotamSearch;
using PilotAppLib.Common;
using Service.Common.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightPlanner.Service.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IFlightDataRepository _dataRepository;
        private readonly INotificationRepository _notificationRepository;


        public IndexModel(
            IFlightDataRepository dataRepository,
            INotificationRepository notificationRepository
            )
        {
            _dataRepository = dataRepository;
            _notificationRepository = notificationRepository;
        }


        public string CurrentTimeUtc
        {
            get {
                return DateTime.Now.ToUniversalTime()
                .ToString("yyyy-MM-dd, HH:mm (UTC)");
            }
        }

        public bool ShowTipsAndTricks
        {
            get {
                var shouldShow = !HttpContext.Session.GetBool("TipsAndTricksShown");
                HttpContext.Session.SetBool("TipsAndTricksShown", true);

                return shouldShow;
            }
        }

        public bool ShowWeightBalanceSection
        {
            get {
                return HttpContext.Items.TryGetValue("Organization", out object organization);
            }
        }

        public string Organization
        {
            get {
                return HttpContext.Items.TryGetValue("Organization", out object organization)
                    ? organization.ToString()
                    : string.Empty;
            }
        }

        public bool SettingsSaved { get; private set; }

        public string TipsAndTricksMessage { get; private set; }

        public Dictionary<IcaoCode, string> Metar { get; private set; }
        public Dictionary<IcaoCode, string> Taf { get; private set; }
        public Dictionary<IcaoCode, List<NotamRecord>> Notam { get; private set; }



        public IActionResult OnGet()
        {
            SettingsSaved = TempData.Remove("SettingsSaved");
            TipsAndTricksMessage = _notificationRepository.GetRandomNotification();

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
