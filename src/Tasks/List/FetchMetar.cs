using PilotAppLib.Clients.MetNorway;
using PilotAppLib.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FlightPlanner.Service.Tasks
{
    public sealed class FetchMetar : TaskBase
    {
        private readonly IReadOnlyList<IcaoCode> _airports;


        public FetchMetar(IReadOnlyList<IcaoCode> airports)
            : base("METAR fetch")
        {
            _airports = airports;
        }


        public sealed override object Run()
        {
            var result = new Dictionary<IcaoCode, string> ();

            using (var client = new MetNorwayClient())
            {
                foreach (IcaoCode airport in _airports)
                {
                    try
                    {
                        string metar = client.FetchMetar(airport.ToString());
                        metar = Regex.Replace(metar, "^[A-Z]{4} ", "");

                        result.Add(airport, metar);
                    }
                    catch (NoDataAvailableException)
                    {
                        // No METAR available for the specific airport, continue
                        continue;
                    }
                }
            }

            return result;
        }
    }
}
