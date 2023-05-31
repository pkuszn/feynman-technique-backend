using System.Linq.Expressions;
using FeynmanTechniqueBackend.Controllers.Base;
using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Controllers.Criteria;
using Microsoft.AspNetCore.Mvc;
using FeynmanTechniqueBackend.Extensions;
using FeynmanTechniqueBackend.Repository.Interfaces;

namespace FeynmanTechniqueBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PartOfSpeechController : BaseEntityReadOnlyController<PartOfSpeech, PartOfSpeechCriteria, int>
    {
        private readonly ILogger<PartOfSpeechController> Logger;
        public PartOfSpeechController(ILogger<PartOfSpeechController> logger, IRepositoryAsync<PartOfSpeech, int> repository)
            : base(repository)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Expression<Func<PartOfSpeech, bool>> PreparePredicate(PartOfSpeechCriteria criteria)
        {
            if (criteria is null)
            {
                Logger.LogError("Get {entity} failed. {criteria} is null or empty.", nameof(PartOfSpeech), nameof(PartOfSpeechCriteria));
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
