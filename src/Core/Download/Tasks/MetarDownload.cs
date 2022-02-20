using FPSE.Core.Parsing;
using FPSE.Core.Types;
using System.Collections.Generic;


namespace FPSE.Core.Download.Tasks
{
    public sealed class MetarDownload : DownloadTask
    {
        private readonly string _metarUrl;
        private readonly IReadOnlyList<IcaoCode> _airports;


        public MetarDownload(string htmlUrl, IReadOnlyList<IcaoCode> airports) 
            : base("METAR download")
        {
            _metarUrl = htmlUrl;
            _airports = airports;
        }


        protected sealed override object Run()
        {
            using (SimpleHttp httpClient = new SimpleHttp())
            {
                string html = httpClient.Get(_metarUrl);
                return MetarFromHtml.ParseDocument(html, _airports);
            }
        }
    }
}
