using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FeynmanTechniqueBackend.Repository
{
    public class RepositoryAsync<E, T>
        : IRepositoryAsync<E, T> where E
            : class, IEntity<T>
    {
        private readonly FeynmanTechniqueBackendContext DbContext;
        public RepositoryAsync(FeynmanTechniqueBackendContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<E>> GetAsync(Expression<Func<E, bool>> expression, CancellationToken cancellationToken)
        {
            return await DbContext.Set<E>().Where(expression).ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<E>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await DbContext.Set<E>().ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<E> GetByIdAsync(T id, CancellationToken cancellationToken)
        {
            E? entity = await DbContext.Set<E>().FirstOrDefaultAsync(f => f.Id.Equals(id), cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<E> PostAsync(E entity, CancellationToken cancellationToken)
        {
            DbContext.Set<E>().Add(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<E> PutAsync(E entity, CancellationToken cancellationToken)
        {
            E? foundEntity = await DbContext.Set<E>().FirstOrDefaultAsync(f => f.Id.Equals(entity.Id), cancellationToken: cancellationToken);
            if (foundEntity == null)
            {
                return null;
            }

            foundEntity = entity;
            await DbContext.SaveChangesAsync(cancellationToken);
            return foundEntity;
        }

        public async Task<bool> DeleteAsync(T id, CancellationToken cancellationToken)
        {
            E? entity = await DbContext.Set<E>().FirstOrDefaultAsync(f => f.Id.Equals(id), cancellationToken: cancellationToken);
            if (entity == null)
            {
                return false;
            }

            DbContext.Set<E>().Remove(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<E>> BulkInsertAsync(IEnumerable<E> entities, CancellationToken cancellationToken)
        {
            DbContext.BulkInsert(entities);
            await DbContext.SaveChangesAsync(cancellationToken);
            return new List<E>(entities);
        }
    }
}
