using FPSE.Core.Parsing;
using FPSE.Core.Types;
using System.Collections.Generic;


namespace FPSE.Core.Download.Tasks
{
    public sealed class NotamDownload : DownloadTask
    {
        private readonly string _notamApiUrl;
        private readonly IReadOnlyList<IcaoCode> _airports;


        public NotamDownload(string notamApiUrl, IReadOnlyList<IcaoCode> airports) 
            : base("NOTAM download")
        {
            _notamApiUrl = notamApiUrl;
            _airports = airports;
        }


        protected sealed override object Run()
        {
            using (SimpleHttp httpClient = new SimpleHttp())
            {
                string postEndpoint = _notamApiUrl
                    + $"?searchType=0"
                    + $"&designatorsForLocation={string.Join(",", _airports)}";

                string json = httpClient.Post(postEndpoint);
                return NotamJsonParser.ParseJson(json);
            }

        }
    }
}
