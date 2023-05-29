using System.Net;
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
		[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(bool))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(object))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(object))]
		public async Task<bool> GetAsync()
		{
			return await ServiceUtilitiesService.GetAsync();
		}

    }
}


