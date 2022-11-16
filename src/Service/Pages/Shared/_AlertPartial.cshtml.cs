using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FlightPlanner.Service.Pages
{
    public class AlertModel : PageModel
    {
        private readonly IReadOnlyDictionary<string, string> 
            _alertClassLookup = new Dictionary<string, string>() {
            { "info", "alert-info" },
            { "success", "alert-success" }
        };


        public string HtmlContent { get; private set; }

        public string AlertClass { get; private set; }


        public AlertModel(string htmlContent, string alertType)
        {
            HtmlContent = htmlContent;
            AlertClass = _alertClassLookup[alertType];
        }
    }
}
