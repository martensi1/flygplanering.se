using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;


namespace FPSE.Core.Download
{
    public class SimpleHttp : IDisposable
    {
        private HttpClient _httpClient;


        public SimpleHttp()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("flygplanering.se", "1.0.0"));
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }


        public string Get(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Invalid URL");

            HttpResponseMessage response = _httpClient.GetAsync(url).Result;
            string content = response.Content.ReadAsStringAsync().Result;

            return content;
        }

        public string Post(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Invalid URL");

            HttpResponseMessage response = _httpClient.PostAsync(url, null).Result;
            string content = response.Content.ReadAsStringAsync().Result;

            return content;
        }
    }
}
