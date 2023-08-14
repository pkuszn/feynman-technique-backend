using System.Linq.Expressions;
using FeynmanTechniqueBackend.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace FeynmanTechniqueBackend.Controllers.Base
{
    public abstract class BaseEntityReadOnlyController<E, C, T> : ControllerBase
        where E : class, IEntity<T>, new()
    {
        private readonly IRepositoryAsync Repository;

        protected BaseEntityReadOnlyController(IRepositoryAsync repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        public async Task<ActionResult<List<E>>> GetAsync([FromBody] C criteria, CancellationToken cancellationToken)
        {
            try
            {
                if (criteria == null)
                {
                    return new List<E>();
                }

                Expression<Func<E, bool>> expression = PreparePredicate(criteria);
                return await Repository.GetWhereAsync(expression, cancellationToken);
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpPost("get")]
        public async Task<ActionResult<List<E>>> GetByPostAsync([FromBody] C criteria, CancellationToken cancellationToken)
        {
            try
            {
                if (criteria == null)
                {
                    return new List<E>();
                }

                Expression<Func<E, bool>> expression = PreparePredicate(criteria);
                return await Repository.GetWhereAsync(expression, cancellationToken);
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<E>>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await Repository.GetAllAsync<E>(cancellationToken: cancellationToken);
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<E>> GetByIdAsync(T id, CancellationToken cancellationToken)
        {
            try
            {
                E? entity = await Repository.GetByIdAsync<E, T>(id, cancellationToken);
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
        protected abstract bool HasLengthLimit(C criteria, out int offset, out int partOfSet);
    }
}
