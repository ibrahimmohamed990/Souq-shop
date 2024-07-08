

namespace Souq.Entities.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? ShippingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTime?  PaymentDate { get; set; }

        // Stripe Properties
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        // User Data
        public string UserName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }

    }
}
