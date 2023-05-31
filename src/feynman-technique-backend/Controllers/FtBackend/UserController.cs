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
    public class UserController : BaseEntityController<User, UserCriteria, int>
    {
        private readonly ILogger<UserController> Logger;
        public UserController(ILogger<UserController> logger, FeynmanTechniqueBackendContext feynmanTechniqueBackendContext) 
            : base(feynmanTechniqueBackendContext)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Expression<Func<User, bool>> PreparePredicate(UserCriteria criteria)
        {
            if (criteria is null)
            {
                Logger.LogError("Get {entity} failed. {criteria} is null or empty.", nameof(User), nameof(UserCriteria));
                return null;
            }

            Expression<Func<User, bool>> expr = f => true;

            if (criteria.IdUser > 0)
            {
                expr = expr.And(a => a.Id == criteria.IdUser);
            }

            if (!string.IsNullOrEmpty(criteria.Name))
            {
                expr = expr.And(a => a.Name.Equals(criteria.Name));
            }

            if (!string.IsNullOrEmpty(criteria.Password))
            {
                expr = expr.And(a => a.Password.Equals(criteria.Password));
            }

            if (criteria.Role > 0)
            {
                expr = expr.And(a => a.Role == criteria.Role);
            }

            if (criteria.CreatedDate > DateTime.MinValue)
            {
                expr = expr.And(a => a.CreatedDate == criteria.CreatedDate);
            }

            return expr;
        }
    }
}
