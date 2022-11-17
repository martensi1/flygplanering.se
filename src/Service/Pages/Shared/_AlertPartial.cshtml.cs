using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace FlightPlanner.Service.Pages
{
    public class AlertModel : PageModel
    {
        private readonly IReadOnlyDictionary<AlertType, string> 
            _alertClassLookup = new Dictionary<AlertType, string>() {
            { AlertType.Info, "alert-info" },
            { AlertType.Success, "alert-success" }
        };


        public string HtmlContent { get; private set; }

        public string AlertClass { get; private set; }


        public AlertModel(string htmlContent, AlertType alertType)
        {
            HtmlContent = htmlContent;
            AlertClass = _alertClassLookup[alertType];
        }
    }
}
