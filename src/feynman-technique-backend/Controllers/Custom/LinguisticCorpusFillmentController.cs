using FeynmanTechniqueBackend.Controllers.Criteria;
using FeynmanTechniqueBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FeynmanTechniqueBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LinguisticCorpusFillmentController : ControllerBase
    {
        private readonly ILinguisticCorpusFillmentService LinguisticCorpusFillmentService;
        public LinguisticCorpusFillmentController(ILinguisticCorpusFillmentService scrapService)
        {
            LinguisticCorpusFillmentService = scrapService ?? throw new ArgumentNullException(nameof(scrapService));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(object))]
        public async Task<bool> PostAsync(ScrapCriteria criteria, CancellationToken cancellationToken) 
            => await LinguisticCorpusFillmentService.PostAsync(criteria, cancellationToken);

        [HttpPost("manual")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(object))]
        public async Task<bool> ManualPostAsync(List<string> words, CancellationToken cancellationToken)
            => await LinguisticCorpusFillmentService.ManualPostAsync(words, cancellationToken);
    }
}

