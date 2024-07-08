using Souq.DataAccessLayer.Context;
using Souq.Entities.Repositories;
using Souq.Entities.Repositories.Identity;
using System.Collections;

namespace Souq.DataAccessLayer.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICategoryRepository CategoryRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IShoppingCartRepository ShoppingCartRepository { get; private set; }
        public IOrderHeaderRepository OrderHeaderRepository { get; private set; }
        public IOrderDetailsRepository OrderDetailsRepository { get; private set; }

        private Hashtable? repositories;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            CategoryRepository = new CategoryRepository(context);
            ProductRepository = new ProductRepository(context);
            ShoppingCartRepository = new ShoppingCartRepository(context);
            OrderHeaderRepository = new OrderHeaderRepository(context);
            OrderDetailsRepository = new OrderDetailsRepository(context);
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            repositories = repositories ?? new Hashtable();
            var entityType = typeof(T).Name;
            if (!repositories.ContainsKey(entityType))
            {
                var repositoryInstance = new GenericRepository<T>(_context);
                repositories.Add(entityType, repositoryInstance);
            }

            return (IGenericRepository<T>)repositories[entityType];
        }
        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
           _context.Dispose();
        }

    }
}
