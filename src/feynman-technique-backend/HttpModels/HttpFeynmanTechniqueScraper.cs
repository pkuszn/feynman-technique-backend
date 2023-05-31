using FeynmanTechniqueBackend.HttpModels.Interfaces;
using Microsoft.Extensions.Options;

namespace FeynmanTechniqueBackend.HttpModels
{
    public class HttpFeynmanTechniqueScraper : IHttpFeynmanTechniqueScraper
    {
        private const string Format = "{Url}/{endpoint}";
        private readonly FeynmanTechniqueScraperConfiguration FeynmanTechniqueScraperConfiguration;
        public HttpFeynmanTechniqueScraper(IOptionsMonitor<FeynmanTechniqueScraperConfiguration> feynmanTechniqueScraperConfiguration)
        {   
            FeynmanTechniqueScraperConfiguration = feynmanTechniqueScraperConfiguration.CurrentValue ?? throw new ArgumentNullException(nameof(feynmanTechniqueScraperConfiguration));
        }

        public string MakeAddress(string endpoint)
        {
            //TODO: REFIT
            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(FeynmanTechniqueScraperConfiguration.Url))
            {
                return string.Empty;
            }

            return string.Format(Format, FeynmanTechniqueScraperConfiguration.Url, endpoint);
        }
    }
}

