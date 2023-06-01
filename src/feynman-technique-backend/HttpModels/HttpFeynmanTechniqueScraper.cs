using FeynmanTechniqueBackend.Configuration;
using FeynmanTechniqueBackend.HttpModels.Interfaces;
using Microsoft.Extensions.Options;
using RestSharp;

namespace FeynmanTechniqueBackend.HttpModels
{
    public class HttpFeynmanTechniqueScraper : IHttpFeynmanTechniqueScraper
    {
        private readonly FeynmanTechniqueScraperOptions ScraperConfiguration;
        public HttpFeynmanTechniqueScraper(IOptionsMonitor<FeynmanTechniqueScraperOptions> scraperConfiguration)
        {
            ScraperConfiguration = scraperConfiguration.CurrentValue ?? throw new ArgumentNullException(nameof(scraperConfiguration));
        }

        public Uri PrepareAddress(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(ScraperConfiguration.Url))
            {
                return new Uri(string.Empty);
            }

            return new Uri($"{ScraperConfiguration.Url}/{endpoint}");
        }

        public RestRequest? PrepareRequest(Uri uri, Method method, object? body = null)
        {
            if (string.IsNullOrEmpty(uri.AbsolutePath) || body == null)
            {
                return null;
            }

            RestRequest request = new(uri, method);

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

