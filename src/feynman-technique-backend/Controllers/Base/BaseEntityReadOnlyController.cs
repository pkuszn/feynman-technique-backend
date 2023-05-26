using System.Linq.Expressions;
using FeynmanTechniqueBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace FeynmanTechniqueBackend.Controllers.Base
{
    public abstract class BaseEntityReadOnlyController<E, C, T> : ControllerBase
        where E: class, IEntity<T>, new()
    {
        public FeynmanTechniqueBackendContext FeynmanTechniqueBackendContext { get; }

        protected BaseEntityReadOnlyController(FeynmanTechniqueBackendContext feynmanTechniqueBackendContext)
        {
            FeynmanTechniqueBackendContext = feynmanTechniqueBackendContext ?? throw new ArgumentNullException(nameof(feynmanTechniqueBackendContext));
        }

        [HttpGet]
        public async Task<ActionResult<List<E>>> GetAsync([FromBody] C criteria)
        {
            try
            {
                if (criteria == null)
                {
                    return new List<E>();
                }

                Expression<Func<E, bool>> expression = PreparePredicate(criteria);
                return await FeynmanTechniqueBackendContext.Set<E>().Where(expression).ToListAsync();
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpPost("get")]
        public async Task<ActionResult<List<E>>> GetByPostAsync([FromBody] C criteria)
        {
            try
            {
                if (criteria == null)
                {
                    return new List<E>();
                }

                Expression<Func<E, bool>> expression = PreparePredicate(criteria);
                return await FeynmanTechniqueBackendContext.Set<E>().Where(expression).ToListAsync();
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<E>>> GetAllAsync()
        {
            try
            {
                return await FeynmanTechniqueBackendContext.Set<E>().ToListAsync();
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<E>> GetByIdAsync(T id)
        {
            try
            {
                E? entity = await FeynmanTechniqueBackendContext.Set<E>().FirstOrDefaultAsync(f => f.Id.Equals(id));
                return entity == null ? StatusCode(StatusCodes.Status404NotFound) : entity;
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        protected StatusCodeResult HandleError(MySqlException exception)
        {
            if (MySqlErrorCode.AccessDenied.Equals(exception.SqlState))
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            if (MySqlErrorCode.AbortingConnection.Equals(exception.SqlState))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            if (MySqlErrorCode.KeyDoesNotExist.Equals(exception.SqlState))
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        protected abstract Expression<Func<E, bool>> PreparePredicate(C criteria);
    }
}
