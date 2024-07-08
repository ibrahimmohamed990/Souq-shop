using Souq.Entities.Models;
using Souq.Entities.Repositories.Identity;

namespace Souq.Entities.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }
        IOrderHeaderRepository OrderHeaderRepository { get; }
        IOrderDetailsRepository OrderDetailsRepository { get; }
        Task<int> Complete();
    }
}
