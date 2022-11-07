using PilotAppLib.Clients.NotamSearch;
using System.Collections.Generic;
using System.Linq;

using FlightPlanner.Core.Types;


namespace FlightPlanner.Core.Tasks
{
    public sealed class FetchNotam : TaskBase
    {
        private readonly IReadOnlyList<IcaoCode> _airports;


        public FetchNotam(IReadOnlyList<IcaoCode> airports) 
            : base("NOTAM fetch")
        {
            _airports = airports;
        }


        protected sealed override object Run()
        {
            var result = new Dictionary<IcaoCode, string> ();
            var icaos = _airports.Select(i => i.ToString()).ToArray();

            using (var client = new NotamSearchClient())
            {
                var notams = new Dictionary<string, string>();//client.FetchNotam(icaos);

                foreach (var notam in notams)
                {
                    result.Add(new IcaoCode(notam.Key), notam.Value);
                }
            }

            return result;
        }
    }
}
