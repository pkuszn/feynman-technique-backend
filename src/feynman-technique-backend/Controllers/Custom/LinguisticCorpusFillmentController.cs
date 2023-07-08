using System.Net;
using FeynmanTechniqueBackend.Controllers.Criteria;
using FeynmanTechniqueBackend.Models;
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
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(bool))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(object))]
        public async Task<List<Word>> PostAsync(ScrapCriteria criteria, CancellationToken cancellationToken)
        {
            return await LinguisticCorpusFillmentService.PostAsync(criteria, cancellationToken);
        }
    }
}

