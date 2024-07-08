
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Souq.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace Souq.Entities.ViewModels
{
    public class OrderHeaderVM
    {
        public OrderHeader OrderHeader { get; set; }
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }

    }
}
