using Microsoft.EntityFrameworkCore;
using Souq.DataAccessLayer.Context;
using Souq.Entities.Repositories;
using System;
using System.Linq.Expressions;

namespace Souq.DataAccessLayer.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? predicate = null, string? Include = null)
        {
            return await ApplySpecifications(predicate, Include).ToListAsync();
        }
        public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>>? predicate = null, string? Include = null, bool tracking = true)
        {
            return await ApplySpecifications(predicate,Include,tracking).FirstOrDefaultAsync();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
            //_dbSet.Attach(entity);
            //_context.Entry(entity).State = EntityState.Modified;
        }
        private IQueryable<T> ApplySpecifications(Expression<Func<T, bool>>? predicate = null, string? Include = null, bool tracking = true)
        {
            IQueryable<T> query = _dbSet;

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            if (predicate is not null)
                query = query.Where(predicate);

            if (Include != null)
            {
                foreach (var item in Include.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(item);
            }
            return query;
        }
    }
}
