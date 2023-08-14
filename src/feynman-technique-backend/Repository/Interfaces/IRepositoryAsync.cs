using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace FeynmanTechniqueBackend.Repository.Interfaces
{
    public interface IRepositoryAsync
    {
        Task<List<E>> GetWhereAsync<E>(Expression<Func<E, bool>> expression, CancellationToken cancellationToken) where E : class;
        Task<List<E>> GetAllAsync<E>(CancellationToken cancellationToken) where E : class;
        Task<List<E>> GetWhereLimitAsync<E>(Expression<Func<E, bool>> expression, int offset, int partOfSet, CancellationToken cancellationToken) where E : class;
        Task<E> GetByIdAsync<E, T>(T id, CancellationToken cancellationToken) where E : class, IEntity<T>;
        Task<E> PostAsync<E>(E entity, CancellationToken cancellationToken) where E : class;
        Task<E> PutAsync<E, T>(E entity, CancellationToken cancellationToken) where E : class, IEntity<T>;
        Task<bool> DeleteAsync<E, T>(T id, CancellationToken cancellationToken) where E : class, IEntity<T>;
        Task<List<E>> BulkInsertAsync<E>(IEnumerable<E> entities, CancellationToken cancellationToken) where E : class;
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}
