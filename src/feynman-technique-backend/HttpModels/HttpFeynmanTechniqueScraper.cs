using FeynmanTechniqueBackend.Configuration;
using FeynmanTechniqueBackend.HttpModels.Interfaces;
using Microsoft.Extensions.Options;

namespace FeynmanTechniqueBackend.HttpModels;

public class HttpFeynmanTechniqueScraper : HttpClient, IHttpFeynmanTechniqueScraper
{
    private readonly FeynmanTechniqueScraperOptions Options;
    public HttpFeynmanTechniqueScraper(IOptionsMonitor<FeynmanTechniqueScraperOptions> options) : base()
    {
        Options = options.CurrentValue ?? throw new ArgumentNullException(nameof(options));
    }

    public Uri PrepareAddress(string endpoint)
    {
        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(Options.Url))
        {
            return new Uri(string.Empty);
        }

        return new Uri($"{Options.Url}/{endpoint}");
    }
}

