using PilotAppLib.Clients.MetNorway;
using PilotAppLib.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FlightPlanner.Service.Tasks
{
    public sealed class FetchTaf : TaskBase
    {
        private readonly IReadOnlyList<IcaoCode> _airports;


        public FetchTaf(IReadOnlyList<IcaoCode> airports) 
            : base("TAF fetch")
        {
            _airports = airports;
        }


        protected sealed override object Run()
        {
            var result = new Dictionary<IcaoCode, string> ();

            using (var client = new MetNorwayClient())
            {
                foreach (IcaoCode airport in _airports)
                {
                    try
                    {
                        string taf = client.FetchTaf(airport);
                        taf = Regex.Replace(taf, "^[A-Z]{4} ", "");

                        result.Add(airport, taf);
                    }
                    catch (NoDataAvailableException)
                    {
                        // No TAF available for the specific airport, continue
                        continue;
                    }
                }
            }

            return result;
        }
    }
}
