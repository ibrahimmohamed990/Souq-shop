using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Souq.Entities.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(OrderDetail.OrderId))]
        public OrderHeader OrderHeader { get; set; }
        public int ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

    }
}
