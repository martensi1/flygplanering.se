using PilotAppLib.Clients.MetNorway;
using System.Collections.Generic;

using FlightPlanner.Core.Types;


namespace FlightPlanner.Core.Tasks
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
                        string metar = client.FetchTaf(airport.ToString());
                        result.Add(airport, metar);
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
