using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FlightPlanner.Service.Pages
{
    public class RulesModel : PageModel
    {
        private readonly ILogger<SettingsModel> _logger;


        public RulesModel(ILogger<SettingsModel> logger)
        {
            _logger = logger;
        }
    }
}
