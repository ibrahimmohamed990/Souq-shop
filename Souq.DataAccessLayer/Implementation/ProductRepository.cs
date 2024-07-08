using Souq.DataAccessLayer.Context;
using Souq.Entities.Models;
using Souq.Entities.Repositories;

namespace Souq.DataAccessLayer.Implementation
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Product> Search(string PName)
        {
            return _context.Products.Where(p => p.Name.ToLower().Trim().Contains(PName.ToLower().Trim()));
        }public IEnumerable<Product> Search(int catId, string PName)
        {
            return _context.Products.Where(p => p.CategoryId == catId && p.Name.ToLower().Trim().Contains(PName.ToLower().Trim()));
        }
    }
}
