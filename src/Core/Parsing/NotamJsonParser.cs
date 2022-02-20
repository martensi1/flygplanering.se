using FPSE.Core.Common;
using FPSE.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace FPSE.Core.Parsing
{
    public static class NotamJsonParser
    {
        private class NotamItem
        {
            [JsonPropertyName("facilityDesignator")]
            public string IcaoCode { get; set; }

            [JsonPropertyName("icaoMessage")]
            public string Message { get; set; }
        }


        public static AirportReports ParseJson(string json)
        {
            JsonDocument document = JsonDocument.Parse(json);
            JsonElement list = document.RootElement.GetProperty("notamList");

            var items = list.EnumerateArray()
                .Select(it => JsonSerializer.Deserialize<NotamItem>(it.GetRawText()))
                .ToList();

            return IterateItems(items);
        }


        private static AirportReports IterateItems(IReadOnlyList<NotamItem> items)
        {
            AirportReports reports = new AirportReports();

            foreach (NotamItem item in items)
            {
                string airport = item.IcaoCode;

                if (!reports.ContainsKey(airport))
                {
                    reports.Add(airport, string.Empty);
                }
                else
                {
                    reports[airport] += 
                        Environment.NewLine +
                        Environment.NewLine;
                }

                reports[airport] += item.Message;
            }

            return reports;
        }
    }
}
