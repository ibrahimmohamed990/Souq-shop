using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Souq.Entities.ViewModels
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}
