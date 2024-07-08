
using System.Linq.Expressions;

namespace Souq.Entities.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? predicate = null, string? Include = null);
        Task<T> GetFirstOrDefault(Expression<Func<T, bool>>? predicate = null, string? Include = null, bool tracking = true);
        Task Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);


    }
}
