using Microsoft.EntityFrameworkCore;
using Souq.DataAccessLayer.Context;
using Souq.Entities.Models;
using Souq.Entities.Repositories;

namespace Souq.DataAccessLayer.Implementation
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void UpdateCart(ShoppingCart cart)
        {
            _context.ShoppingCarts.Update(cart);
        }
        public int CartsCount(string userId)
        {
            return _context.ShoppingCarts.Count(x => x.UserId == userId);
        }
    }
}
