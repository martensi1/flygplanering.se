using Microsoft.AspNetCore.Http;
using System.Linq;

namespace FlightPlanner.Service.Pages
{
    public static class SessionUtil
    {
        public static void SetFlag(HttpContext context, SeassionFlag flag)
        {
            if (IsAvailable(context))
            {
                string key = flag.ToString();
                context.Session.Set(key, new byte[0] { });
            }
        }

        public static bool GetFlag(HttpContext context, SeassionFlag flag)
        {
            if (IsAvailable(context))
            {
                string key = flag.ToString();
                return context.Session.Keys.Contains(key);
            }

            return false;
        }

        private static bool IsAvailable(HttpContext context)
        {
            return context.Session != null;
        }
    }
}
