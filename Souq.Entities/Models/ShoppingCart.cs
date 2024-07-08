using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Souq.Entities.Models
{
    public class ShoppingCart
    {
        public int ShoppingCartId { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }

        [Range(1, 100, ErrorMessage = "You must enter value between 1 to 100.")]
        public int Count { get; set; }

        public string UserId { get; set; }
        //[ForeignKey("UserId")]
        //[ValidateNever]
        //public ApplicationUser ApplicationUser { get; set; }

    }
}
