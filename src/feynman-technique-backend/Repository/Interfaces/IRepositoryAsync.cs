using System.Linq.Expressions;

namespace FeynmanTechniqueBackend.Repository.Interfaces
{
    public interface IRepositoryAsync<E, T>
        where E : class, IEntity<T>
    {
        Task<List<E>> GetAsync(Expression<Func<E, bool>> expression, CancellationToken cancellationToken);
        Task<List<E>> GetAllAsync(CancellationToken cancellationToken);
        Task<E> GetByIdAsync(T id, CancellationToken cancellationToken);
        Task<E> PostAsync(E entity, CancellationToken cancellationToken);
        Task<E> PutAsync(E entity, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(T id, CancellationToken cancellationToken);
        Task<List<E>> BulkInsertAsync(IEnumerable<E> entities, CancellationToken cancellationToken);
    }
}
