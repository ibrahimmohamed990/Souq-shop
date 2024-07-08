
using Souq.Entities.Models;

namespace Souq.Entities.Repositories
{
    public interface IOrderHeaderRepository:IGenericRepository<OrderHeader>
    {
        void UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus);
        OrderHeader GetLastOrder(string userId);
    }
}
