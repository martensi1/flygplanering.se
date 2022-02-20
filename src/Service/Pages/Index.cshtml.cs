using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using FPSE.Core;
using FPSE.Core.Types;
using Microsoft.Net.Http.Headers;


namespace FPSE.Service.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IFlightDataCache _flightDataCache;

        public DateTime LastGet { get; private set; }

        public AirportReports CurrentMetar;
        public AirportReports CurrentTaf;
        public AirportReports CurrentNotam;


        public IndexModel(
            ILogger<IndexModel> logger,
            IFlightDataCache flightDataCache
            )
        {
            _logger = logger;
            _flightDataCache = flightDataCache;
        }


        public void OnGet()
        {
            LastGet = DateTime.Now;

            SetCacheHeaders();
            WaitForData(10000, 50);
        }


        private void SetCacheHeaders()
        {
            Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
            Response.Headers[HeaderNames.Expires] = "0";
            Response.Headers[HeaderNames.Pragma] = "no-cache";
        }

        private void WaitForData(short timeoutMs, short retryTimeMs)
        {
            for (int i = 0; i < (timeoutMs / retryTimeMs); i++)
            {
                if (_flightDataCache.IsDataAvailable())
                {
                    CurrentMetar = _flightDataCache.GetCurrentMetar();
                    CurrentTaf = _flightDataCache.GetCurrentTaf();
                    CurrentNotam = _flightDataCache.GetCurrentNotam();

                    break;
                }

                Thread.Sleep(retryTimeMs);
            }
        }
    }
}
