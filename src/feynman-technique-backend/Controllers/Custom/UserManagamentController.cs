using FeynmanTechniqueBackend.Controllers.Criteria;
using FeynmanTechniqueBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(object))]
        public async Task<bool> PostAsync(ValidateUserCriteria criteria, CancellationToken cancellationToken) 
            => await UserManagementService.PostAsync(criteria, cancellationToken);
    }
}
