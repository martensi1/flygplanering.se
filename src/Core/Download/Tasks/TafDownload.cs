using FPSE.Core.Parsing;
using FPSE.Core.Types;
using System.Collections.Generic;


namespace FPSE.Core.Download.Tasks
{
    public sealed class TafDownload : DownloadTask
    {
        private readonly string _tafUrl;
        private readonly IReadOnlyList<IcaoCode> _airports;


        public TafDownload(string htmlUrl, IReadOnlyList<IcaoCode> airports) 
            : base("TAF download")
        {
            _tafUrl = htmlUrl;
            _airports = airports;
        }


        protected sealed override object Run()
        {
            using (SimpleHttp httpClient = new SimpleHttp())
            {
                string html = httpClient.Get(_tafUrl);
                return TafFromHtml.ParseDocument(html, _airports);
            }
        }
    }
}
