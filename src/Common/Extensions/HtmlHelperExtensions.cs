using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightPlanner.Service.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static string GenerateRandomId(this IHtmlHelper _)
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
