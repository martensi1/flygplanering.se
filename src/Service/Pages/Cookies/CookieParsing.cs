using System.Collections.Generic;
using System.Linq;
using FlightPlanner.Core.Types;

namespace FlightPlanner.Service
{
    public class CookieParsing
    {
        public static IReadOnlyList<IcaoCode> ToAirportList(string cookieValue, IReadOnlyList<IcaoCode> defaultValue)
        {
            var result = CookieToAirportList(cookieValue);
            return (result ?? defaultValue);
        }
        
        public static string FromAirportList(IReadOnlyList<IcaoCode> list)
        {
            return string.Join(",", list.Select(icao 
                => icao.ToString()).ToArray());
        }

        
        private static IReadOnlyList<IcaoCode> CookieToAirportList(string cookieValue)
        {
            if (cookieValue == null)
                return null;

            
            List<IcaoCode> result = new List<IcaoCode>();
            string[] strings = cookieValue.Split(',');

            for (int i = 0; i < strings.Length; i++)
            {
                string icao = strings[i]
                    .Trim();
                
                if (!IcaoCode.IsStringValid(icao))
                    return null;

                result.Add(icao);
            }
            

            return result;
        }
    }
}
