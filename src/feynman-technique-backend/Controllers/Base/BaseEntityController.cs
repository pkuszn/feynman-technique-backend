using System.Linq.Expressions;
using FeynmanTechniqueBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace FeynmanTechniqueBackend.Controllers.Base
{
    public abstract class BaseEntityController<E, C, T> : ControllerBase
        where E: class, IEntity<T>, new()
    {
        public FeynmanTechniqueBackendContext FeynmanTechniqueBackendContext { get; }
        protected BaseEntityController(FeynmanTechniqueBackendContext feynmanTechniqueBackendContext)
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

        [HttpPost]
        public async Task<ActionResult<E>> PostAsync([FromBody] E entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }

                FeynmanTechniqueBackendContext.Set<E>().Add(entity);
                await FeynmanTechniqueBackendContext.SaveChangesAsync();
                return entity;
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpPut]
        public async Task<ActionResult<E>> PutAsync([FromBody] E entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest();
                }

                E? foundEntity = await FeynmanTechniqueBackendContext.Set<E>().FirstOrDefaultAsync(f => f.Id.Equals(entity.Id));
                if (foundEntity == null)
                {
                    return NotFound();
                }

                foundEntity = entity;
                await FeynmanTechniqueBackendContext.SaveChangesAsync();
                return foundEntity;
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteAsync(T id)
        {
            try
            {
                E? entity = await FeynmanTechniqueBackendContext.Set<E>().FirstOrDefaultAsync(f => f.Id.Equals(id));
                if (entity == null)
                {
                    return NotFound();
                }

                FeynmanTechniqueBackendContext.Set<E>().Remove(entity);
                await FeynmanTechniqueBackendContext.SaveChangesAsync();
                return true;
            }
            catch (MySqlException exception)
            {
                return HandleError(exception);
            }
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<List<E>>> BulkInsert([FromBody] IEnumerable<E> entities)
        {
            try
            {
                if (!entities.Any())
                {
                    return new List<E>();
                }

                FeynmanTechniqueBackendContext.BulkInsert(entities);
                await FeynmanTechniqueBackendContext.SaveChangesAsync();
                return new List<E>(entities);
            }  
            catch(MySqlException exception)
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
    }
}
