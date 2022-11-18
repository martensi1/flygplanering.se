using FlightPlanner.Service.Models;
using PilotAppLib.Clients.NotamSearch;
using System.Collections.Generic;
using System.Linq;

namespace FlightPlanner.Service.Tasks
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
            var icaoCodes = _airports.Select(i => i.ToString()).ToArray();
            
            try
            {
                return FetchNotams(icaoCodes);
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                // If the fetch fails, rethrow the exception (will be caught by the TaskScheduler)
                throw ex;
            }
        }
        
        private Dictionary<IcaoCode, List<NotamRecord>> FetchNotams(string[] icaoCodes)
        {
            var result = new Dictionary<IcaoCode, List<NotamRecord>>();

            using (var client = new NotamSearchClient())
            {
                var fetchedNotams = client.FetchNotams(icaoCodes);

                foreach ((string icaoCode, List<NotamRecord> aerodromeNotams) in fetchedNotams)
                {
                    result.Add(new IcaoCode(icaoCode), aerodromeNotams);
                }
            }

            return result;
        }
    }
}
