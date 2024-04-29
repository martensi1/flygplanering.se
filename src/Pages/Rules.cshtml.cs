using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FlightPlanner.Service.Pages
{
    public class SettingsModel : PageModel
    {
        private readonly ILogger<SettingsModel> _logger;


        public SettingsModel(ILogger<SettingsModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnPost()
        {
            TempData["SettingsSaved"] = true;
            _logger.LogInformation("Settings saved, redirecting user to index page");

            return RedirectToPage("/Index");
        }
    }
}
