using FeynmanTechniqueBackend.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace FeynmanTechniqueBackend.Controllers.Base
{
    public abstract class BaseEntityController<E, C, T> : BaseEntityReadOnlyController<E, C, T>
        where E : class, IEntity<T>, new()
    {
        protected BaseEntityController(IRepositoryAsync repository) : base(repository) { }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<E>> PostAsync([FromBody] E entity, CancellationToken cancellationToken)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }

                return CreatedAtAction(nameof(PostAsync), await Repository.PostAsync(entity, cancellationToken));
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<E>> PutAsync([FromBody] E entity, CancellationToken cancellationToken)
        {
            try
            {
                if (entity == null)
                {
                    return NoContent();
                }

                return Ok(await Repository.PutAsync<E, T>(entity, cancellationToken));
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteAsync([FromRoute] T id, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await Repository.DeleteAsync<E, T>(id, cancellationToken));
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpPost("bulk")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<E>>> BulkInsert([FromBody] IEnumerable<E> entities, CancellationToken cancellationToken)
        {
            try
            {
                if (!entities.Any())
                {
                    return new List<E>();
                }

                return CreatedAtAction(nameof(BulkInsert), await Repository.BulkInsertAsync(entities, cancellationToken));
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
    }
}
