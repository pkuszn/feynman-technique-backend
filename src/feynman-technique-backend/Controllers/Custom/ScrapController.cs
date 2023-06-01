using System.Net;
using FeynmanTechniqueBackend.DtoModels;
using FeynmanTechniqueBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FeynmanTechniqueBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScrapController : ControllerBase
    {
        private readonly IScrapService ScrapService;
        public ScrapController(IScrapService scrapService)
        {
            ScrapService = scrapService ?? throw new ArgumentNullException(nameof(scrapService));
        }

        [HttpPost("many")]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(bool))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(object))]
        public async Task<ScrapDto> PostAsync(ScrapCriteria criteria, CancellationToken cancellationToken)
        {
            return await ScrapService.PostAsync(criteria, cancellationToken);
        }
    }
}

