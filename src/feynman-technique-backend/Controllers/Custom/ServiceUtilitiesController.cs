using FeynmanTechniqueBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FeynmanTechniqueBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceUtilitiesController : ControllerBase
    {
        private readonly IServiceUtilitiesService ServiceUtilitiesService;
        public ServiceUtilitiesController(IServiceUtilitiesService serviceUtilitiesService)
        {
            ServiceUtilitiesService = serviceUtilitiesService ?? throw new ArgumentNullException(nameof(serviceUtilitiesService));
        }

        [HttpGet("duplicates")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(object))]
        public async Task<bool> GetAsync(CancellationToken cancellationToken) => await ServiceUtilitiesService.GetAsync(cancellationToken);
    }
}


