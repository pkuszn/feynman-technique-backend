using FeynmanTechniqueBackend.Controllers.Criteria;
using FeynmanTechniqueBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FeynmanTechniqueBackend.Controllers.Custom
{
    [ApiController]
    [Route("[controller]")]
    public class UserManagamentController : ControllerBase
    {
        private readonly IUserManagementService UserManagementService; 
        public UserManagamentController(IUserManagementService userManagementService)
        {
            UserManagementService = userManagementService ?? throw new ArgumentNullException(nameof(userManagementService));
        }

        [HttpPost("authenticate")]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(bool))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(object))]
        public async Task<bool> PostAsync(ValidateUserCriteria criteria, CancellationToken cancellationToken)
        {
            return await UserManagementService.PostAsync(criteria, cancellationToken);
        }
    }
}
