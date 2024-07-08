using Souq.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Souq.Entities.ViewModels
{
    public class OrderVM
    {
        public OrderHeaderVM OrderHeader { get; set; }
        public IEnumerable<OrderDetail>? OrderDetails { get; set; }
    }
}
