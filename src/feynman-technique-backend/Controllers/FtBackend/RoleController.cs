using System.Linq.Expressions;
using FeynmanTechniqueBackend.Controllers.Base;
using FeynmanTechniqueBackend.Controllers.Criteria;
using FeynmanTechniqueBackend.Extensions;
using FeynmanTechniqueBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeynmanTechniqueBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : BaseEntityReadOnlyController<Role, RoleCriteria, int>
    {
        private readonly ILogger<RoleController> Logger;
        public RoleController(ILogger<RoleController> logger, FeynmanTechniqueBackendContext feynmanTechniqueBackendContext) 
            : base(feynmanTechniqueBackendContext)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Expression<Func<Role, bool>> PreparePredicate(RoleCriteria criteria)
        {
            if (criteria is null)
            {
                Logger.LogError("Get {entity} failed. {criteria} is null or empty.", nameof(Role), nameof(RoleCriteria));
                return null;
            }

            Expression<Func<Role, bool>> expr = f => true;

            if (criteria.IdRole > 0)
            {
                expr = expr.And(a => a.Id == criteria.IdRole);
            }
            
            if (!string.IsNullOrEmpty(criteria.Name))
            {
                expr = expr.And(a => a.Name.Equals(criteria.Name));
            }

            return expr;
        }
    }
}
