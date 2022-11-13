using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace FlightPlanner.Service.Pages
{
    public class SettingsModel : PageModel
    {
        private readonly ILogger<ErrorModel> _logger;


        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);


        public SettingsModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }


        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}