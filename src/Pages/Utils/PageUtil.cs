using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System;

namespace FlightPlanner.Service.Pages
{
    public static class PageUtil
    {
        public static string GetCrsfToken(HttpContext context)
        {
            var antiforgery = (IAntiforgery)context.RequestServices
                .GetService(typeof(IAntiforgery));

            return antiforgery.GetAndStoreTokens(context).RequestToken;
        }

        public static string GenerateRandomElementId()
        {
            var guid = Guid.NewGuid();
            guid = EnsureStartsWithLetter(guid);

            return guid.ToString();
        }

        private static Guid EnsureStartsWithLetter(Guid guid)
        {
            var bytes = guid.ToByteArray();

            if ((bytes[3] & 0xf0) < 0xa0)
            {
                bytes[3] |= 0xc0;
                return new System.Guid(bytes);
            }
            return guid;
        }
    }
}
