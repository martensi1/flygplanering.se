using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace FlightPlanner.Service.Pages
{
    public class AlertPartialModel : PageModel
    {
        private readonly IReadOnlyDictionary<string, string>
            _alertClassLookup = new Dictionary<string, string>() {
            { "info", "alert-info" },
            { "success", "alert-success" }
        };



        public string AlertClass { get; private set; }

        public string HtmlContent { get; private set; }


        public AlertPartialModel(string alertType, string htmlContent)
        {
            if (alertType == null)
            {
                throw new ArgumentNullException(nameof(alertType));
            }

            if (!_alertClassLookup.ContainsKey(alertType))
            {
                throw new ArgumentException("Invalid alert type");
            }

            AlertClass = _alertClassLookup[alertType];
            HtmlContent = htmlContent;
        }
    }
}
