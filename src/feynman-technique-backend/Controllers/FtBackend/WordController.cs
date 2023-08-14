using System.Linq.Expressions;
using FeynmanTechniqueBackend.Constants;
using FeynmanTechniqueBackend.Controllers.Base;
using FeynmanTechniqueBackend.Controllers.Criteria;
using FeynmanTechniqueBackend.Extensions;
using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FeynmanTechniqueBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WordController : BaseEntityController<Word, WordCriteria, int>
    {
        private readonly ILogger<WordController> Logger;
        public WordController(ILogger<WordController> logger, IRepositoryAsync repository)
            : base(repository)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override bool HasLengthLimit(WordCriteria criteria, out int offset, out int partOfSet)
        {
            offset = 0;
            partOfSet = 0;

            if (!criteria.Offset.HasValue && !criteria.PartOfSet.HasValue)
            {
                return false;
            }

            if (!criteria.Offset.HasValue)
            {
                offset = Query.Pagination.DefaultOffset;
            }
            else
            {
                offset = criteria.Offset.Value;
            }

            if (!criteria.PartOfSet.HasValue)
            {
                partOfSet = Query.Pagination.DefaultPartOfSet;
            }
            else
            {
                partOfSet = criteria.PartOfSet.Value;
            }

            return true;
        }

        protected override Expression<Func<Word, bool>> PreparePredicate(WordCriteria criteria)
        {
            if (criteria is null)
            {
                Logger.LogError("Get {entity} failed. {criteria} is null or empty.", nameof(Word), nameof(WordCriteria));
                return null;
            }

            Expression<Func<Word, bool>> expr = f => true;

            if (criteria?.IdWord?.Length > 0)
            {
                expr = expr.And(a => criteria.IdWord.Contains(a.Id));
            }

            if (criteria?.Name?.Length > 0)
            {
                expr = expr.And(a => criteria.Name.Contains(a.Name));
            }

            if (criteria?.PartOfSpeech?.Length > 0)
            {
                expr = expr.And(a => criteria.PartOfSpeech.Contains(a.PartOfSpeech));
            }

            if (criteria?.CreatedDate != null)
            {
                expr = expr.And(a => a.CreatedDate == criteria.CreatedDate);
            }

            if (criteria?.Context?.Length > 0)
            {
                expr = expr.And(a => criteria.Context.Contains(a.Context));
            }

            if (criteria?.Link?.Length > 0)
            {
                expr = expr.And(a => criteria.Link.Contains(a.Link));
            }

            if (criteria?.CreatedDate == null && criteria?.FromTime != null)
            {
                expr = expr.And(a => a.CreatedDate >= criteria.FromTime);
            }

            if (criteria?.CreatedDate == null && criteria?.ToTime != null)
            {
                expr = expr.And(a => a.CreatedDate <= criteria.ToTime);
            }

            return expr;
        }
    }
}