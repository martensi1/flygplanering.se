using FlightPlanner.Core.Types;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlightPlanner.Service
{
    public class SettingsCookie
    {
        [JsonPropertyName("METAR")]
        public List<IcaoCode> MetarAirports { get; set; } = new List<IcaoCode>() {
            "ESMS",
            "ESGJ",
            "ESGR",
            "ESMX",
            "ESMT",
            "ESGG",
            "ESSL",
            "ESSV"
        };

        [JsonPropertyName("TAF")]
        public List<IcaoCode> TafAirports { get; set; } = new List<IcaoCode>() {
            "ESMS",
            "ESGJ",
            "ESGR",
            "ESMX",
            "ESMT",
            "ESGG",
            "ESSL",
            "ESSV"
        };

        [JsonPropertyName("NOTAM")]
        public List<IcaoCode> NotamAirports { get; set; } = new List<IcaoCode>() {
            "ESGJ"
        };



        private const string CookieName = "fpl-airports";

        public static SettingsCookie CreateFrom(HttpRequest httpRequest)
        {
            string jsonString = httpRequest.Cookies[CookieName];

            if (jsonString == null)
            {
                // No cookie exists, use default values instead
                return new SettingsCookie();
            }

            try
            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new IcaoCodeListConverter());

                return JsonSerializer.Deserialize<SettingsCookie>(jsonString, options);
            }
            catch (JsonException)
            {
                // Failed to deserialize request cookie, use default values instead
                return new SettingsCookie();
            }
        }
   
        public void WriteTo(HttpResponse httpResponse)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new IcaoCodeListConverter());

            string jsonString = JsonSerializer.Serialize(this, options);

            httpResponse.Cookies.Append(CookieName, jsonString, new CookieOptions() {
                IsEssential = true,
            });
        }

        
        private class IcaoCodeListConverter : JsonConverter<List<IcaoCode>>
        {
            public override List<IcaoCode> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var list = new List<IcaoCode>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;

                    if (reader.TokenType == JsonTokenType.String)
                    {
                        var code = reader.GetString();

                        if (IcaoCode.IsStringValid(code))
                            list.Add(code);
                    }
                }

                return list;
            }

            public override void Write(Utf8JsonWriter writer, List<IcaoCode> value, JsonSerializerOptions options)
            {
                writer.WriteStartArray();

                foreach (var code in value)
                {
                    writer.WriteStringValue(code.ToString());
                }

                writer.WriteEndArray();
            }
        }
    }
}
