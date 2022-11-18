using FlightPlanner.Service.Models;
using PilotAppLib.Clients.MetNorway;
using System.Collections.Generic;

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


        protected sealed override object Run()
        {
            var result = new Dictionary<IcaoCode, string> ();

            using (var client = new MetNorwayClient())
            {
                foreach (IcaoCode airport in _airports)
                {
                    try
                    {
                        string metar = client.FetchMetar(airport.ToString());
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
