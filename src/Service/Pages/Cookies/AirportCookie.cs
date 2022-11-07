using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FlightPlanner.Service
{
    public class AirportsCookie
    {
        public IReadOnlyList<string> Airports { get; }


        public AirportsCookie(string cookieValue, IReadOnlyList<string> defaultValue)
        {
            var result = ValueToList(cookieValue);
            Airports = result ?? defaultValue;
        }

        public string ToCookie()
        {
            return string.Join(",", Airports.ToArray());
        }


        private static IReadOnlyList<string> ValueToList(string cookieValue)
        {
            if (cookieValue == null)
                return null;


            string[] airports = cookieValue.Split(',');

            for (int i = 0; i < airports.Length; i++)
            {
                string icao = airports[i]
                    .Trim();

                if (!Regex.IsMatch(icao, "^[A-Z]{4}$"))
                    return null;

                airports[i] = icao;
            }


            return airports.ToList();
        }
    }
}
