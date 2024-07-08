using Souq.DataAccessLayer.Context;
using Souq.Entities.Models;
using Souq.Entities.Repositories;
using Souq.Utilities;


namespace Souq.DataAccessLayer.Implementation
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public OrderHeader GetLastOrder(string userId)
        {
            return _context.OrderHeaders.OrderByDescending(x => x.Id).FirstOrDefault(x => x.UserId == userId && x.OrderStatus == SD.Pending);
        }

        public void UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus)
        {
            var orderInDB = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if(orderInDB is not null)
            {
                orderInDB.OrderStatus = OrderStatus;
                if(orderInDB.PaymentStatus is not null)
                    orderInDB.PaymentStatus = PaymentStatus;
            }

        }
    }
}
