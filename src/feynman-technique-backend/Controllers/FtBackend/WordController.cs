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
    public class WordController : BaseEntityController<Word, WordCriteria, int>
    {
        private readonly ILogger<WordController> Logger;
        public WordController(ILogger<WordController> logger, FeynmanTechniqueBackendContext feynmanTechniqueBackendContext) 
            : base(feynmanTechniqueBackendContext)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Expression<Func<Word, bool>> PreparePredicate(WordCriteria criteria)
        {
            if (criteria is null)
            {
                return null;
            }

            Expression<Func<Word, bool>> expr = f => true;

            if (criteria.IdWord > 0)
            {
                expr = expr.And(a => a.Id == criteria.IdWord);
            }

            if (!string.IsNullOrEmpty(criteria.Name))
            {
                expr = expr.And(a => a.Name.Equals(criteria.Name));
            }

            if (criteria.PartOfSpeech > 0)
            {
                expr = expr.And(a => a.PartOfSpeech == criteria.PartOfSpeech);
            }

            if (criteria.CreatedDate != DateTime.MinValue)
            {
                expr = expr.And(a => a.CreatedDate == criteria.CreatedDate);
            }

            if (!string.IsNullOrEmpty(criteria.Context))
            {
                expr = expr.And(a => a.Context.Equals(criteria.Context));
            }

            if (!string.IsNullOrEmpty(criteria.Link))
            {
                expr = expr.And(a => a.Link.Equals(criteria.Link));
            }

            return expr;
        }
    }
}