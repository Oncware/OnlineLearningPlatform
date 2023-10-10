using JustLearn1.Data;
using JustLearn1.Interface;
using JustLearn1.Models.Interfaces;

namespace JustLearn1.Models.Services
{
    public class ProductRepository : Repository<Product> , IProductRepository
    {
        private JustDbContext dbContext;
        public ProductRepository(JustDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return dbContext.Products;
        }
        public Product? GetProductDetail(int id)
        {
            return dbContext.Products.FirstOrDefault(p => p.Id == id);
        }


        public IEnumerable<Product> GetTrendingProducts()
        {
            return dbContext.Products.Where(p => p.IsTrendingProduct);
        }
        public void Update(Product obj)
        {

            var objFromDb = dbContext.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.Price = obj.Price;
                objFromDb.Detail = obj.Detail;
                objFromDb.Category = obj.Category;
                objFromDb.IsTrendingProduct = obj.IsTrendingProduct;
                objFromDb.CategoryId = obj.CategoryId;
                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
        
