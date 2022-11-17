using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FlightPlanner.Service.Pages
{
    public static class CookieUtil
    {
        public static bool IsConsentPending(HttpContext context)
        {
            var consentFeature = context.Features.Get<ITrackingConsentFeature>();
            return !consentFeature?.CanTrack ?? false;
        }

        public static string CreateConsentCookie(HttpContext context)
        {
            var consentFeature = context.Features.Get<ITrackingConsentFeature>();
            return consentFeature?.CreateConsentCookie();
        }

        public static bool HasConsent(HttpContext context)
        {
            var consentFeature = context.Features.Get<ITrackingConsentFeature>();
            return consentFeature?.HasConsent ?? false;
        }
        
        public static void GrantConsent(HttpContext context)
        {
            var consentFeature = context.Features.Get<ITrackingConsentFeature>();
            consentFeature.GrantConsent();
        }

        public static void WithdrawConsent(HttpContext context)
        {
            var consentFeature = context.Features.Get<ITrackingConsentFeature>();
            consentFeature.WithdrawConsent();
        }
    }
}
