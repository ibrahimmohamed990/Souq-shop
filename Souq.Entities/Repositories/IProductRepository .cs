using Souq.Entities.Models;

namespace Souq.Entities.Repositories
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        IEnumerable<Product> Search(string PName);
        IEnumerable<Product> Search(int id, string PName);
    }
}
