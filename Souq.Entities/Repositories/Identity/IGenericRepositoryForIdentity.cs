
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace Souq.Entities.Repositories.Identity
{
    public interface IGenericRepositoryForIdentity<T> where T : IdentityUser
    {
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? predicate = null, string? Include = null);
        Task<T> GetFirstOrDefault(Expression<Func<T, bool>>? predicate = null, string? Include = null, bool tracking = true);
        Task Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);


    }
}
