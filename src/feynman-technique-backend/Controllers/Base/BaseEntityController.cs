using System.Linq.Expressions;
using FeynmanTechniqueBackend.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace FeynmanTechniqueBackend.Controllers.Base
{
    public abstract class BaseEntityController<E, C, T> : ControllerBase
        where E : class, IEntity<T>, new()
    {
        public IRepositoryAsync Repository { get; }
        protected BaseEntityController(IRepositoryAsync repository)
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
                bool hasLengthLimit = HasLengthLimit(criteria, out int offset, out int partOfSet);
                return hasLengthLimit 
                    ? await Repository.GetWhereLimitAsync(expression, offset, partOfSet, cancellationToken) 
                    : await Repository.GetWhereAsync(expression, cancellationToken);
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
                return await Repository.GetAllAsync<E>(cancellationToken);
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<E>> GetByIdAsync([FromRoute] T id, CancellationToken cancellationToken)
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

        [HttpPost]
        public async Task<ActionResult<E>> PostAsync([FromBody] E entity, CancellationToken cancellationToken)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }

                return await Repository.PostAsync(entity, cancellationToken);
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpPut]
        public async Task<ActionResult<E>> PutAsync([FromBody] E entity, CancellationToken cancellationToken)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }

                return await Repository.PutAsync<E, T>(entity, cancellationToken);
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync([FromRoute] T id, CancellationToken cancellationToken)
        {
            try
            {
                return await Repository.DeleteAsync<E, T>(id, cancellationToken);
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<List<E>>> BulkInsert([FromBody] IEnumerable<E> entities, CancellationToken cancellationToken)
        {
            try
            {
                if (!entities.Any())
                {
                    return new List<E>();
                }

                return await Repository.BulkInsertAsync(entities, cancellationToken);
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetAmountOfEntriesAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await Repository.GetAmountOfEntriesAsync<E>(cancellationToken);
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }
        
        [HttpPost("{column}/get")]
        public async Task<ActionResult<List<object>>> GetByColumnAsync([FromRoute] string column, CancellationToken cancellationToken)
        {
            try
            {
                Microsoft.EntityFrameworkCore.Metadata.IProperty? property = Repository.TryGetColumnName<E>(column);
                if (property == null)
                {
                    return NotFound();
                }

                return await Repository.GetByColumnAsync<E>(property, cancellationToken);
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
            if (MySqlErrorCode.BadTable.Equals(exception.SqlState))
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            if (MySqlErrorCode.ColumnAccessDenied.Equals(exception.SqlState))
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            if (MySqlErrorCode.WrongTableName.Equals(exception.SqlState))
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            if (MySqlErrorCode.WrongKeyColumn.Equals(exception.SqlState))
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            if (MySqlErrorCode.BulkCopyFailed.Equals(exception.SqlState))
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        protected abstract Expression<Func<E, bool>> PreparePredicate(C criteria);
        protected abstract bool HasLengthLimit(C criteria, out int offset, out int partOfSet);
    }
}
