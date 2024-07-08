
namespace Souq.Entities.ViewModels
{
    public class ProductToDelete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
