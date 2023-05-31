using FeynmanTechniqueBackend.DtoModels;
using FeynmanTechniqueBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FeynmanTechniqueBackend.Controllers
{
    public class ScrapController : ControllerBase
   {
        private readonly IScrapService ScrapService;
        public ScrapController(IScrapService scrapService)
        {
            ScrapService = scrapService ?? throw new ArgumentNullException(nameof(scrapService));
        }

        [HttpPost("/many")]
        public async Task<ScrapDto> PostAsync(ScrapCriteria criteria, CancellationToken cancellationToken)
        {
            return await ScrapService.PostAsync(criteria, cancellationToken);
        }
    }
}

