using FPSE.Core.Common;
using FPSE.Core.Types;
using System.Collections.Generic;


namespace FPSE.Core.Parsing
{
    public static class TafFromHtml
    {
        public static AirportReports ParseDocument(string html, IReadOnlyList<IcaoCode> airports)
        {
            AirportReports reports = new AirportReports();

            foreach (IcaoCode icao in airports)
            {
                string match = SimpleRegex.FindPattern(icao + @".*?\d{6}Z \d{4}/\d{4} .*?=", html);
                if (string.IsNullOrWhiteSpace(match))
                {
                    continue;
                }

                match = SimpleRegex.FindPattern(@"\d{6}Z \d{4}/\d{4} .*?=", match);
                if (string.IsNullOrWhiteSpace(match))
                {
                    continue;
                }

                reports[icao] = TrimAndProcessMatch(match, icao);
            }

            return reports;
        }


        private static string TrimAndProcessMatch(string match, IcaoCode icao)
        {
            match = match.Replace("\n", "");
            match = match.Replace("\t", "");
            match = match.Replace("\r", "");
            match = match.Trim();

            if (match.StartsWith(icao.ToString()))
                match = match.Substring(4);

            return match;
        }
    }
}
