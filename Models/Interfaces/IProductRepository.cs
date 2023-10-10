using JustLearn1.Repository.IRepository;
using JustLearn1.ViewModel;

namespace JustLearn1.Models.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetTrendingProducts();
        Product? GetProductDetail(int id);
        void Update(Product obj);
    }
}
