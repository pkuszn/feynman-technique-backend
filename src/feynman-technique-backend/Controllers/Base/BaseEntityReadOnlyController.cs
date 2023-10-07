using System.Linq.Expressions;
using FeynmanTechniqueBackend.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace FeynmanTechniqueBackend.Controllers.Base
{
    public abstract class BaseEntityReadOnlyController<E, C, T> : ControllerBase
        where E : class, IEntity<T>, new()
    {
        protected readonly IRepositoryAsync Repository;

        protected BaseEntityReadOnlyController(IRepositoryAsync repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<E>>> GetAsync([FromBody] C criteria, CancellationToken cancellationToken)
        {
            try
            {
                if (criteria == null)
                {
                    return new List<E>();
                }

                Expression<Func<E, bool>> expression = PreparePredicate(criteria);
                return Ok(await Repository.GetWhereAsync(expression, cancellationToken));
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpPost("get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<E>>> GetByPostAsync([FromBody] C criteria, CancellationToken cancellationToken)
        {
            try
            {
                if (criteria == null)
                {
                    return new List<E>();
                }

                Expression<Func<E, bool>> expression = PreparePredicate(criteria);
                bool hasLengthLimit = HasLengthLimit(criteria, out int offset, out int partOfSet);
                List<E> entities = hasLengthLimit
                    ? await Repository.GetWhereLimitAsync(expression, offset, partOfSet, cancellationToken)
                    : await Repository.GetWhereAsync(expression, cancellationToken);
                return (entities?.Count ?? 0) == 0 ? NotFound() : Ok(entities);
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<E>>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await Repository.GetAllAsync<E>(cancellationToken));
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<E>> GetByIdAsync([FromRoute] T id, CancellationToken cancellationToken)
        {
            try
            {
                E? entity = await Repository.GetByIdAsync<E, T>(id, cancellationToken);
                return entity == null ? NotFound() : Ok(entity);
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetAmountOfEntriesAsync(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await Repository.GetAmountOfEntriesAsync<E>(cancellationToken));
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpPost("{column}/get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<object>>> GetByColumnAsync([FromRoute] string column, CancellationToken cancellationToken)
        {
            try
            {
                Microsoft.EntityFrameworkCore.Metadata.IProperty? property = Repository.TryGetColumnName<E>(column);
                if (property == null)
                {
                    return BadRequest();
                }

                List<object> entities = await Repository.GetByColumnAsync<E>(property, cancellationToken);
                return (entities?.Count ?? 0) == 0 ? NotFound() : Ok(entities);
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        private StatusCodeResult HandleError(MySqlException exception)
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
