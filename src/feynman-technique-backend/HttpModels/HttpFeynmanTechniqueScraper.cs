using FeynmanTechniqueBackend.HttpModels.Interfaces;
using Microsoft.Extensions.Options;
using RestSharp;

namespace FeynmanTechniqueBackend.HttpModels
{
    public class HttpFeynmanTechniqueScraper : IHttpFeynmanTechniqueScraper
    {
        private readonly FeynmanTechniqueScraperConfiguration ScraperConfiguration;
        private const string Format = "{Url}/{endpoint}";
        private const int Timeout = 30;
        public HttpFeynmanTechniqueScraper(IOptionsMonitor<FeynmanTechniqueScraperConfiguration> scraperConfiguration)
        {   
            ScraperConfiguration = scraperConfiguration.CurrentValue ?? throw new ArgumentNullException(nameof(scraperConfiguration));
        }

        public Uri PrepareAddress(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(ScraperConfiguration.Url))
            {
                return new Uri(string.Empty);
            }

            return new Uri(string.Format(Format, ScraperConfiguration.Url, endpoint));
        }

        public RestRequest? PrepareRequest(Uri uri, Method method, object? body = null)
        {
            if (string.IsNullOrEmpty(uri.AbsolutePath) || body == null)
            {
                return null;
            }

            RestRequest request = new(uri, method)
            {
                Timeout = Timeout
            };

            if (body != null)
            {
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(body);
            }

            return request;
        }
    }
}

