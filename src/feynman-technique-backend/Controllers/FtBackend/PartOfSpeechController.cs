using System.Linq.Expressions;
using FeynmanTechniqueBackend.Controllers.Base;
using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Controllers.Criteria;
using Microsoft.AspNetCore.Mvc;
using FeynmanTechniqueBackend.Extensions;

namespace FeynmanTechniqueBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PartOfSpeechController : BaseEntityReadOnlyController<PartOfSpeech, PartOfSpeechCriteria, int>
    {
        private readonly ILogger<PartOfSpeechController> Logger;
        public PartOfSpeechController(ILogger<PartOfSpeechController> logger, FeynmanTechniqueBackendContext feynmanTechniqueBackendContext)
            : base(feynmanTechniqueBackendContext)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Expression<Func<PartOfSpeech, bool>> PreparePredicate(PartOfSpeechCriteria criteria)
        {
            if (criteria is null)
            {
                return null;
            }

            Expression<Func<PartOfSpeech, bool>> expr = f => true;

            if (criteria.IdPartOfSpeech > 0)
            {
                expr = expr.And(a => a.Id == criteria.IdPartOfSpeech);
            }

            if (!string.IsNullOrEmpty(criteria.Name))
            {
                expr = expr.And(a => a.Name.Equals(criteria.Name)); 
            }

            return expr;
        }
    }
}
