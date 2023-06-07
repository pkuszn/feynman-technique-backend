using FeynmanTechniqueBackend.Configuration;
using FeynmanTechniqueBackend.HttpModels.Interfaces;
using Microsoft.Extensions.Options;
using RestSharp;

namespace FeynmanTechniqueBackend.HttpModels
{
    public class HttpFeynmanTechniqueCore : HttpClient, IHttpFeynmanTechniqueCore
    {
        private readonly FeynmanTechniqueCoreOptions Options;
        public HttpFeynmanTechniqueCore(IOptionsMonitor<FeynmanTechniqueCoreOptions> options) : base()
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
}