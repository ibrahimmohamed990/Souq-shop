using Souq.DataAccessLayer.Context;
using Souq.Entities.Models;
using Souq.Entities.Repositories;


namespace Souq.DataAccessLayer.Implementation
{
    public class OrderDetailsRepository : GenericRepository<OrderDetail>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
