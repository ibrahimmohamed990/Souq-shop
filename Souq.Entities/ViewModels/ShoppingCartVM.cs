using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Souq.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Souq.Entities.ViewModels
{
    public class ShoppingCartVM
    {
        [ValidateNever]
        public IEnumerable<ShoppingCart> CartsList { get; set; }
        [ValidateNever]
        public decimal TotalPriceForCarts { get; set; }
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }

    }
}
