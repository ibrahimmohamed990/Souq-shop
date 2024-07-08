using Microsoft.AspNetCore.Mvc.Rendering;
using Souq.Entities.Models;

namespace Souq.Entities.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
    }
}
