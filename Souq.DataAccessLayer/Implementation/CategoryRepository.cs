using Souq.DataAccessLayer.Context;
using Souq.Entities.Models;
using Souq.Entities.Repositories;


namespace Souq.DataAccessLayer.Implementation
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
