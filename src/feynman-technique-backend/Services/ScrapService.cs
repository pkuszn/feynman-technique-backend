using FeynmanTechniqueBackend.DtoModels;
using FeynmanTechniqueBackend.HttpModels.Interfaces;
using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Services.Interfaces;
using RestSharp;
using static FeynmanTechniqueBackend.Constants.Addresses;

namespace FeynmanTechniqueBackend.Services
{
    public class ScrapService : IScrapService
    {
        private readonly ILogger<ScrapService> Logger;
        private readonly FeynmanTechniqueBackendContext FeynmanTechniqueBackendContext;
        private readonly IHttpFeynmanTechniqueScraper HttpFeynmanTechniqueScraper;

        public ScrapService(
            ILogger<ScrapService> logger, 
            FeynmanTechniqueBackendContext feynmanTechniqueBackendContext,
            IHttpFeynmanTechniqueScraper httpFeynmanTechniqueScraper)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            FeynmanTechniqueBackendContext = feynmanTechniqueBackendContext ?? throw new ArgumentNullException(nameof(feynmanTechniqueBackendContext));    
            HttpFeynmanTechniqueScraper = httpFeynmanTechniqueScraper ?? throw new ArgumentNullException(nameof(httpFeynmanTechniqueScraper));
        }

        public async Task<ScrapDto> GetAsync(ScrapCriteria c, CancellationToken cancellationToken)
        {
            //TODO: VALIDATION
            ///TODO: Dokończyć

            string request = HttpFeynmanTechniqueScraper.MakeAddress(FeynmanTechniqueScraperUrl.Many);
            RestClient client = new();
            RestRequest restRequest = new(request);
            RestResponse response = await client.GetAsync(restRequest, cancellationToken);
            return new();
        }
    }
}
