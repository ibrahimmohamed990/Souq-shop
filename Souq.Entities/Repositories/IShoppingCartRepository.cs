
using Souq.Entities.Models;

namespace Souq.Entities.Repositories
{
    public interface IShoppingCartRepository:IGenericRepository<ShoppingCart>
    {
        public void UpdateCart(ShoppingCart cart);
        public int CartsCount(string userId);
    }
}
