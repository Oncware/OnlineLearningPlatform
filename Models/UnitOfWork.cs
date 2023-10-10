using JustLearn1.Data;
using JustLearn1.Repository.IRepository;
using JustLearn1.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using JustLearn1.Models.Services;
using JustLearn1.Models;

namespace JustLearn1.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private JustDbContext _dbContex;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }

        public UnitOfWork(JustDbContext dbContex)
        {
            _dbContex = dbContex;
            Category = new CategoryRepository(_dbContex);
            Product = new ProductRepository(_dbContex);
        }
        public void Save()
        {
            _dbContex.SaveChanges();
        }

        public Product? GetProductDetail(int id)
        {
            return _dbContex.Products.FirstOrDefault(p => p.Id == id);
        }
    }
}
