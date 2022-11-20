using PilotAppLib.Common;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FlightPlanner.Service.Config
{
    public static class AirportConfig
    {
        public static List<IcaoCode> ReadToList()
        {
            var airports = new List<IcaoCode>();
            var json = File.ReadAllText("wwwroot/airports.json");

            var document = JsonDocument.Parse(json);
            var airportsElement = document.RootElement.GetProperty("airports");

            foreach (var airportElement in airportsElement.EnumerateArray())
            {
                var icaoCode = airportElement.GetProperty("icao").GetString();
                airports.Add(icaoCode);
            }

            return airports;
        }
    }
}
